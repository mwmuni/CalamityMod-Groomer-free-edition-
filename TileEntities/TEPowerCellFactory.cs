using System.IO;
using System.Reflection;
using CalamityMod.CalPlayer;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Packets;
using CalamityMod.Tiles.DraedonStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.TileEntities
{
    public class TEPowerCellFactory : TEBasePlaceable
    {
        public Vector2 Center => Position.ToWorldCoordinates(8f * PowerCellFactory.Width, 8f * PowerCellFactory.Height);
        public long Time = 0;

        protected override int PlacementTileWidth => PowerCellFactory.Width;
        protected override int PlacementTileHeight => PowerCellFactory.Height;

        internal short Stack_Internal = 0;

        public short CellStack
        {
            get => Stack_Internal;
            set
            {
                Stack_Internal = value;
                SendSyncPacket();
            }
        }

        private long CycleFrameCounter
        {
            get
            {
                long totalCycleTime = PowerCellFactory.BetweenCellDowntime + PowerCellFactory.TotalFrames * PowerCellFactory.AnimationFramerate;
                return Time % totalCycleTime;
            }
        }

        private bool IsCellFrame
        {
            get
            {
                long magicFrame = PowerCellFactory.BetweenCellDowntime + PowerCellFactory.CellCreateFrame * PowerCellFactory.AnimationFramerate + PowerCellFactory.MagicFrameDelay;
                return CycleFrameCounter == magicFrame;
            }
        }

        // Property which allows anyone to get the current animation frame of this specific factory.
        public int AnimationFrame
        {
            get
            {
                int f = (int)CycleFrameCounter;

                // The animation sticks on the last frame throughout the entire downtime period.
                if (f < PowerCellFactory.BetweenCellDowntime)
                    return PowerCellFactory.TotalFrames - 1;

                // Remove the starting downtime period for the framerate divisor calculation.
                return (f - PowerCellFactory.BetweenCellDowntime) / PowerCellFactory.AnimationFramerate;
            }
        }

        // This guarantees that this tile entity will not persist if not placed directly on the top left corner of a Power Cell Factory tile.
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<PowerCellFactory>() && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        public override void Update()
        {
            // CIT 7OCT2024: Power Cell Factories now produce power cells faster when sleeping or using Journey's time rate multiplier.
            for (int t = 0; t < Main.desiredWorldTilesUpdateRate; t++)
            {
                ++Time;
                int maxCellStack = ModContent.GetModItem(ModContent.ItemType<DraedonPowerCell>()).Item.maxStack;
                if (IsCellFrame && CellStack < maxCellStack)
                    // The property setter will automatically send the necessary packet.
                    CellStack++;
            }
        }

        // If this factory breaks, anyone who's viewing it is no longer viewing it.
        public override void OnKill()
        {
            foreach (Player p in Main.ActivePlayers)
            {
                // Use reflection to stop TML from spitting an error here.
                // Try-catching will not stop this error, TML will print it to console anyway. The error is harmless.
                ModPlayer[] mpStorageArray = (ModPlayer[])typeof(Player).GetField("modPlayers", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(p);
                if (mpStorageArray.Length == 0)
                    continue;

                CalamityPlayer mp = p.Calamity();
                if (mp.CurrentlyViewedFactoryID == ID)
                    mp.CurrentlyViewedFactoryID = -1;
            }
        }

        public override void SaveData(TagCompound tag)
        {
            tag["time"] = Time;
            tag["cells"] = Stack_Internal;
        }

        public override void LoadData(TagCompound tag)
        {
            Time = tag.GetLong("time");
            Stack_Internal = tag.GetShort("cells");
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(Time);
            writer.Write(Stack_Internal);
        }

        public override void NetReceive(BinaryReader reader)
        {
            Time = reader.ReadInt64();
            Stack_Internal = reader.ReadInt16();
        }

        private void SendSyncPacket()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            TEPowerCellFactoryPacket.Send(this, Time, Stack_Internal);
        }
    }
}

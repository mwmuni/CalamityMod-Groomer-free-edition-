using System.IO;
using System.Reflection;
using CalamityMod.CalPlayer;
using CalamityMod.Packets;
using CalamityMod.Tiles.DraedonStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.TileEntities
{
    public class TELabHologramProjector : TEBasePlaceable
    {
        public const float PopupDistance = 560f;

        public Vector2 Center => Position.ToWorldCoordinates(LabHologramProjector.Width * 8f, LabHologramProjector.Height * 8f);
        public bool PoppingUp = false;

        protected override int PlacementTileWidth => LabHologramProjector.Width;
        protected override int PlacementTileHeight => LabHologramProjector.Height;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];

            int style = 0, alt = 0;
            TileObjectData.GetTileInfo(tile, ref style, ref alt);
            TileObjectData data = TileObjectData.GetTileData(tile.TileType, style, alt);

            int sheetSquare = 16 + data.CoordinatePadding;
            int FrameX = tile.TileFrameX / sheetSquare % data.Width;
            int FrameY = tile.TileFrameY / sheetSquare % data.Height;

            return tile.HasTile && tile.TileType == ModContent.TileType<LabHologramProjector>() && FrameX == 0 && FrameY == 0;
        }

        // Check if the hologram should become visible.
        public override void Update()
        {
            bool wasPoppingUp = PoppingUp;

            // Stop popping up by default.
            PoppingUp = false;

            // But check if a player is nearby.
            // If one is, pop up.
            Vector2 projectorCenterPos = Center;
            float distSQ = PopupDistance * PopupDistance;
            foreach (Player p in Main.ActivePlayers)
            {
                // The lack of a dead check is intentional. Dead players keep hologram boxes on for amusement.

                if (p.DistanceSQ(projectorCenterPos) < distSQ)
                {
                    PoppingUp = true;
                    break;
                }
            }

            if (PoppingUp != wasPoppingUp)
                SendSyncPacket();
        }

        // If this projector breaks, anyone who's viewing it is no longer viewing it.
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
                if (mp.CurrentlyViewedHologramID == ID)
                {
                    mp.CurrentlyViewedHologramID = -1;
                    mp.CurrentlyViewedHologramText = string.Empty;
                }
            }
        }

        public override void NetSend(BinaryWriter writer) => writer.Write(PoppingUp);
        public override void NetReceive(BinaryReader reader) => PoppingUp = reader.ReadBoolean();

        private void SendSyncPacket()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            TELabHologramProjectorPacket.Send(this, PoppingUp);
        }
    }
}

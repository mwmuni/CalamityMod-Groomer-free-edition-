using CalamityMod.Packets;
using CalamityMod.Tiles.Furniture.Paintings;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityMod.TileEntities
{
    public class TECanvasPainting : TEBasePlaceable
    {
        public Vector2 framePosition = new Vector2(0, 0);
        public float scale = 1f;

        protected override int PlacementTileWidth => 5;
        protected override int PlacementTileHeight => 5;
        protected override int PlacementOffsetX => -1;
        protected override int PlacementOffsetY => -1;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && TileLoader.GetTile(tile.TileType) is BaseCanvasPainting && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(framePosition.X);
            writer.Write(framePosition.Y);
            writer.Write(scale);
        }

        public override void NetReceive(BinaryReader reader)
        {
            framePosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            scale = reader.ReadSingle();
        }

        public override void SaveData(TagCompound tag)
        {
            tag.Add("posX", framePosition.X);
            tag.Add("posY", framePosition.Y);
            tag.Add("scale", scale);
        }

        public override void LoadData(TagCompound tag)
        {
            framePosition = new Vector2(tag.Get<float>("posX"), tag.Get<float>("posY"));
            scale = tag.Get<float>("scale");
        }

        public void SendSyncPacket()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;

            TECanvasPaintingPacket.Send(this, framePosition.X, framePosition.Y, scale);
        }
    }
}

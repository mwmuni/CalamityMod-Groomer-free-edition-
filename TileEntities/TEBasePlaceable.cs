using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public abstract class TEBasePlaceable : ModTileEntity
    {
        protected abstract int PlacementTileWidth { get; }
        protected abstract int PlacementTileHeight { get; }

        protected virtual int PlacementOffsetX => 0;
        protected virtual int PlacementOffsetY => 0;

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            int placementX = i + PlacementOffsetX;
            int placementY = j + PlacementOffsetY;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, placementX, placementY, PlacementTileWidth, PlacementTileHeight);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, placementX, placementY, Type);
                return -1;
            }

            return Place(placementX, placementY);
        }

        public override void OnNetPlace() => NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
    }
}

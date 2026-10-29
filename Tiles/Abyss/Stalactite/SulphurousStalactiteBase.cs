using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Abyss.Stalactite
{
    public abstract class SulphurousStalactiteBase : ModTile
    {
        protected abstract bool IsStalactite { get; }
        protected abstract int TileHeight { get; }

        public override void SetStaticDefaults()
        {
            bool isStalactite = IsStalactite;
            int tileHeight = TileHeight;

            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = tileHeight;
            TileObjectData.newTile.Origin = new Point16(0, isStalactite ? 0 : tileHeight - 1);

            if (isStalactite)
            {
                TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            }
            else
            {
                TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            }

            TileObjectData.newTile.UsesCustomCanPlace = true;
            int[] coordinateHeights = new int[tileHeight];
            for (int i = 0; i < coordinateHeights.Length; i++)
                coordinateHeights[i] = 16;
            TileObjectData.newTile.CoordinateHeights = coordinateHeights;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.DrawYOffset = isStalactite ? -2 : 2;
            TileObjectData.addTile(Type);

            string localizationKey = isStalactite ? "Tiles.Stalactite" : "Tiles.Stalagmite";
            AddMapEntry(new Color(57, 48, 83), CalamityUtils.GetText(localizationKey));
            DustType = (int)CalamityDusts.SulphurousSeaAcid;

            base.SetStaticDefaults();
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }

    public abstract class SulphurousStalactiteTileBase : SulphurousStalactiteBase
    {
        protected sealed override bool IsStalactite => true;
    }

    public abstract class SulphurousStalacmiteTileBase : SulphurousStalactiteBase
    {
        protected sealed override bool IsStalactite => false;
    }
}

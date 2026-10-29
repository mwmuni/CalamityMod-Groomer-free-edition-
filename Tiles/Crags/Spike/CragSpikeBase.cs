using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Crags.Spike
{
    public abstract class CragSpikeBase : ModTile
    {
        protected abstract int SpikeWidth { get; }
        protected abstract int SpikeHeight { get; }
        protected abstract Point16 SpikeOrigin { get; }
        protected abstract int DrawOffset { get; }
        protected abstract bool HangsFromCeiling { get; }
        protected virtual bool UsesStyle2x2 => false;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;

            if (UsesStyle2x2)
                TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            else
            {
                TileObjectData.newTile.Width = SpikeWidth;
                TileObjectData.newTile.Height = SpikeHeight;
                int[] coordinateHeights = new int[SpikeHeight];
                for (int row = 0; row < coordinateHeights.Length; row++)
                    coordinateHeights[row] = 16;
                TileObjectData.newTile.CoordinateHeights = coordinateHeights;
            }

            TileObjectData.newTile.Origin = SpikeOrigin;
            TileObjectData.newTile.StyleWrapLimit = 36;
            if (HangsFromCeiling)
            {
                TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
                TileObjectData.newTile.AnchorBottom = default(AnchorData);
            }
            else
                TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.DrawYOffset = DrawOffset;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(43, 23, 46));
            DustType = DustID.Ambient_DarkBrown;
            HitSound = SoundID.Tink;
        }
    }
}

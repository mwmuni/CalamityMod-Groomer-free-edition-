using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class AgedLaboratoryTerminal : LaboratoryDustDecorBase
    {
        public override void SetStaticDefaults()
        {
            SetFurnitureDefaults();
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(0, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.LavaDeath = false;

            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            SetHorizontalAlternates();
            TileObjectData.addTile(Type);

            AddMapEntry(Color.DarkSlateGray, CalamityUtils.GetText("Tiles.Terminal"));
        }

    }
}

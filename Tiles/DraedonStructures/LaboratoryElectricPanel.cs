using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LaboratoryElectricPanel : LaboratoryDecorBase
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.newTile.AnchorWall = true;

            TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;

            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);

            AddMapEntry(Color.DimGray, CalamityUtils.GetText("Tiles.ElectricPanel"));
            DustType = DustID.Iron;
        }

    }
}

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LaboratoryContainmentBox : LaboratoryDustDecorBase
    {
        public override void SetStaticDefaults()
        {
            SetFurnitureDefaults();
            Main.tileSolidTop[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;

            SetHorizontalAlternates();
            TileObjectData.addTile(Type);

            AddMapEntry(new Color(82, 67, 66), CalamityUtils.GetText("Tiles.ContainmentBox"));
        }

    }
}

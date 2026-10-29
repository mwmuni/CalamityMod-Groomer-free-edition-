using Microsoft.Xna.Framework;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class AgedLaboratoryServer : LaboratoryGlowDustDecorBase
    {
        protected override string GlowTexturePath => "CalamityMod/Tiles/DraedonStructures/AgedLaboratoryServerGlow";

        public override void SetStaticDefaults()
        {
            SetFurnitureDefaults();
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16 };
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;

            SetHorizontalAlternates();
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(82, 67, 66), CalamityUtils.GetText("Tiles.Server"));
        }
    }
}

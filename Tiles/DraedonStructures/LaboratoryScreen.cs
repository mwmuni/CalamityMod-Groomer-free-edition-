using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.DraedonStructures
{
    public class LaboratoryScreen : LaboratoryGlowDecorBase
    {
        protected override string GlowTexturePath => "CalamityMod/Tiles/DraedonStructures/LaboratoryScreenGlow";

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            AddMapEntry(Color.DarkGray, CalamityUtils.GetText("Tiles.Screen"));
            TileID.Sets.FramesOnKillWall[Type] = true;
            DustType = DustID.Iron;
        }

    }
}

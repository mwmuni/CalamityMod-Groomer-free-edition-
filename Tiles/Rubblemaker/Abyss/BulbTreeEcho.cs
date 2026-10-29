using CalamityMod.Items.Placeables.Abyss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.Abyss
{
    public class BulbTree1Echo : BaseRubblemakerGlowEcho
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/BulbTree1";
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.WaterDeath = false;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(52, 124, 153));
            DustType = DustID.Water;

            RegisterItemDrop(ModContent.ItemType<Voidstone>());
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<Voidstone>(), Type, 0);

            base.SetStaticDefaults();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.098f;
            g = 0.318f;
            b = 0.484f;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }

    public class BulbTree2Echo : BulbTree1Echo
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/BulbTree2";
    }

    public class BulbTree3Echo : BulbTree1Echo
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/BulbTree3";
    }
}

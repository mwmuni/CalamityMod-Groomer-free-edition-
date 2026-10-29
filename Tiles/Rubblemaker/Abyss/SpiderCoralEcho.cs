using CalamityMod.Items.Placeables.Abyss;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.Rubblemaker.Abyss
{
    public class SpiderCoral1Echo : BaseRubblemakerGlowEcho
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral1";
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(82, 49, 27));
            DustType = DustID.Sand;

            RegisterItemDrop(ModContent.ItemType<PyreMantle>());
            FlexibleTileWand.RubblePlacementLarge.AddVariations(ModContent.ItemType<PyreMantle>(), Type, 0);

            base.SetStaticDefaults();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 190f / 400f;
            g = 48f / 400f;
            b = 30f / 400f;
        }
    }

    public class SpiderCoral2Echo : SpiderCoral1Echo
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral2";
    }

    public class SpiderCoral3Echo : SpiderCoral1Echo
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral3";
    }

    public class SpiderCoral4Echo : SpiderCoral1Echo
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral4";
    }

    public class SpiderCoral5Echo : SpiderCoral1Echo
    {
        public override string Texture => "CalamityMod/Tiles/Abyss/AbyssAmbient/SpiderCoral5";
    }
}

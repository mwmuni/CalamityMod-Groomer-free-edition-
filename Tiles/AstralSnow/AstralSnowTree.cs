using CalamityMod.Items.Potions.Food;
using CalamityMod.Tiles.Astral;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralSnow
{
    public class AstralSnowTree : AstralTreeBase
    {
        protected override int GrowthGroundTile => ModContent.TileType<AstralSnow>();
        protected override string TexturePath => "CalamityMod/Tiles/AstralSnow/AstralSnowTree";
        protected override float MinimumGlowBrightness => 0.35f;
        protected override int SaplingType => ModContent.TileType<AstralSnowTreeSapling>();
        protected override int FirstFruitType => ModContent.ItemType<Cometfruit>();
        protected override int SecondFruitType => ModContent.ItemType<Mangosteen>();
    }
}

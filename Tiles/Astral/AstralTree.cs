using CalamityMod.Items.Potions.Food;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Astral
{
    public class AstralTree : AstralTreeBase
    {
        protected override int GrowthGroundTile => ModContent.TileType<AstralGrass>();
        protected override string TexturePath => "CalamityMod/Tiles/Astral/AstralTree";
        protected override float MinimumGlowBrightness => 0.1f;
        protected override int SaplingType => ModContent.TileType<AstralTreeSapling>();
        protected override int FirstFruitType => ModContent.ItemType<Barberry>();
        protected override int SecondFruitType => ModContent.ItemType<Lotus>();
        protected override bool UsesEnchantedNightcrawlerWhenRaining => true;
    }
}

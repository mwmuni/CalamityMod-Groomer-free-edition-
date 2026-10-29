using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Tiles.SunkenSea
{
    public class AlgalPrismTorch : BaseCustomTorchTile<Items.Placeables.Furniture.AlgalPrismTorch>
    {
        protected override Color TorchDustColor => new(150, 255, 190);
        protected override Vector3 TorchLightColor => new(0.7f, 1.2f, 0.9f);
        protected override string FlameTexturePath => "CalamityMod/Tiles/SunkenSea/AlgalPrismTorchFlame";
        protected override int FlameSparkDustType => DustID.GreenTorch;
        protected override bool WaterImmune => true;
        protected override bool LavaImmune => true;
        protected override bool IsInTorchLuckBiome(Player player) => player.Calamity().ZoneSunkenSea;
    }
}

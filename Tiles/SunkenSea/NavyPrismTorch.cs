using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Tiles.SunkenSea
{
    public class NavyPrismTorch : BaseCustomTorchTile<Items.Placeables.Furniture.NavyPrismTorch>
    {
        protected override Color TorchDustColor => new(170, 255, 255);
        protected override Vector3 TorchLightColor => new(0.8f, 1.2f, 1.2f);
        protected override string FlameTexturePath => "CalamityMod/Tiles/SunkenSea/NavyPrismTorchFlame";
        protected override int FlameSparkDustType => DustID.BlueTorch;
        protected override bool WaterImmune => true;
        protected override bool LavaImmune => true;
        protected override bool IsInTorchLuckBiome(Player player) => player.Calamity().ZoneSunkenSea;
    }
}

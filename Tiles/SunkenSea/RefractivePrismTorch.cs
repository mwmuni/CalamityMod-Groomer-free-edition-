using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Tiles.SunkenSea
{
    public class RefractivePrismTorch : BaseCustomTorchTile<Items.Placeables.Furniture.RefractivePrismTorch>
    {
        protected override Color TorchDustColor => new(210, 190, 255);
        protected override Vector3 TorchLightColor => new(1f, 0.9f, 1.2f);
        protected override string FlameTexturePath => "CalamityMod/Tiles/SunkenSea/RefractivePrismTorchFlame";
        protected override int FlameSparkDustType => DustID.PurpleTorch;
        protected override bool WaterImmune => true;
        protected override bool LavaImmune => true;
        protected override bool IsInTorchLuckBiome(Player player) => player.Calamity().ZoneSunkenSea;
    }
}

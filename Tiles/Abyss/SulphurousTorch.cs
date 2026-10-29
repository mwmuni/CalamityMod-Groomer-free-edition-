using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Tiles.Abyss
{
    public class SulphurousTorch : BaseCustomTorchTile<Items.Placeables.Furniture.SulphurousTorch>
    {
        protected override Color TorchDustColor => new(190, 255, 60);
        protected override Vector3 TorchLightColor => new(0.9f, 1.2f, 0.3f);
        protected override string FlameTexturePath => "CalamityMod/Tiles/Abyss/SulphurousTorchFlame";
        protected override int FlameSparkDustType => DustID.YellowTorch;
        protected override int AlternateFlameSparkDustType => DustID.GreenTorch;
        protected override bool WaterImmune => true;
        protected override bool IsInTorchLuckBiome(Player player) => player.Calamity().ZoneSulphur;
    }
}

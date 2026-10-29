using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Tiles.Abyss
{
    public class KelpTorch : BaseCustomTorchTile<Items.Placeables.Furniture.KelpTorch>
    {
        protected override Color TorchDustColor => new(160, 210, 60);
        protected override Vector3 TorchLightColor => new(1.8f, 1.5f, 0.4f);
        protected override string FlameTexturePath => "CalamityMod/Tiles/Abyss/KelpTorchFlame";
        protected override int FlameSparkDustType => DustID.YellowTorch;
        protected override bool WaterImmune => true;
        protected override bool LavaImmune => true;
        protected override bool IsInTorchLuckBiome(Player player) => player.Calamity().ZoneAbyss;
    }
}

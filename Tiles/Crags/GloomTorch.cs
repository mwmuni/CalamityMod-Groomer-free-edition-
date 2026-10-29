using CalamityMod.Tiles.BaseTiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityMod.Tiles.Crags
{
    public class GloomTorch : BaseCustomTorchTile<Items.Placeables.Furniture.GloomTorch>
    {
        protected override Color TorchDustColor => new(190, 255, 60);
        protected override Vector3 TorchLightColor => new(0.5f, 0.75f, 1.2f);
        protected override string FlameTexturePath => "CalamityMod/Tiles/Crags/GloomTorchFlame";
        protected override int FlameSparkDustType => DustID.BlueTorch;
        protected override bool LavaImmune => true;
        protected override bool IsInTorchLuckBiome(Player player) => player.Calamity().ZoneCalamity;
    }
}

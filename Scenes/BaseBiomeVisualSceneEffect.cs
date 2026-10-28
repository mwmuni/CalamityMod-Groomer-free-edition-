using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public abstract class BaseBiomeVisualSceneEffect : ModSceneEffect
    {
        // Keep this optional so subclasses of existing scene types can retain their prior no-op behavior.
        protected virtual string BiomeVisualKey => string.Empty;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (string.IsNullOrEmpty(BiomeVisualKey))
                return;

            player.ManageSpecialBiomeVisuals(BiomeVisualKey, isActive);
        }
    }
}

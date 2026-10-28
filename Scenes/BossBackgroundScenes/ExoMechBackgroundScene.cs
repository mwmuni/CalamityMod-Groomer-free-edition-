using CalamityMod.Skies;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class ExoMechBackgroundScene : BaseBiomeVisualSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        protected override string BiomeVisualKey => "CalamityMod:ExoMechs";

        public override bool IsSceneEffectActive(Player player) => ExoMechsSky.CanSkyBeActive || player.Calamity().monolithExoShader > 0;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            base.SpecialVisuals(player, isActive);
            if (isActive)
                SkyManager.Instance.Activate("CalamityMod:ExoMechs", player.Center);
            else
                SkyManager.Instance.Deactivate("CalamityMod:ExoMechs");
        }
    }
}

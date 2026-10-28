using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class AstralMonolithScene : BaseBiomeVisualSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        protected override string BiomeVisualKey => "CalamityMod:Astral";

        public override bool IsSceneEffectActive(Player player) => Main.LocalPlayer.Calamity().monolithAstralShader > 0;
    }
}

using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class MonolithAccursedScene : BaseBiomeVisualSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        protected override string BiomeVisualKey => "CalamityMod:MonolithAccursed";

        public override bool IsSceneEffectActive(Player player) => player.Calamity().monolithAccursedShader > 0;
    }
}

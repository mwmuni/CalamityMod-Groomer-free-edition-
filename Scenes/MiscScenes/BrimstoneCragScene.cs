using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class BrimstoneCragScene : BaseBiomeVisualSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        protected override string BiomeVisualKey => "CalamityMod:BrimstoneCrag";

        public override bool IsSceneEffectActive(Player player) => player.Calamity().ZoneCalamity;
    }
}

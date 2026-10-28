using CalamityMod.NPCs.CalClone;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class CalamitasCloneBackgroundScene : BaseBiomeVisualSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<CalamitasClone>());

        protected override string BiomeVisualKey => "CalamityMod:CalamitasRun3";
    }
}

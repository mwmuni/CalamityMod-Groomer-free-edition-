using CalamityMod.NPCs.Crabulon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DrunkCrabBackgroundScene : BaseBiomeVisualSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        protected override string BiomeVisualKey => "CalamityMod:DrunkCrabulon";

        public override bool IsSceneEffectActive(Player player)
        {
            if (Main.zenithWorld && NPC.AnyNPCs(ModContent.NPCType<Crabulon>()))
                return true;
            return false;
        }
    }
}

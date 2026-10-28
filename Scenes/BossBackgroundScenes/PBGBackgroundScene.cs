using CalamityMod.NPCs.PlaguebringerGoliath;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class PBGBackgroundScene : BaseBiomeVisualSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        protected override string BiomeVisualKey => "CalamityMod:PlaguebringerGoliath";

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<PlaguebringerGoliath>()) || Main.LocalPlayer.Calamity().monolithPlagueShader > 0;
    }
}

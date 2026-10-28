using CalamityMod.NPCs.Yharon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class YharonBackgroundScene : BaseBiomeVisualSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        protected override string BiomeVisualKey => "CalamityMod:Yharon";

        public override bool IsSceneEffectActive(Player player) => NPC.AnyNPCs(ModContent.NPCType<Yharon>()) || Main.LocalPlayer.Calamity().monolithYharonShader > 0;
    }
}

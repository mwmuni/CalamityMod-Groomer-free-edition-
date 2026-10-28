using CalamityMod.NPCs.Leviathan;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class LeviathanBackgroundScene : BaseBiomeVisualSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        protected override string BiomeVisualKey => "CalamityMod:Leviathan";

        public override bool IsSceneEffectActive(Player player) => (Main.zenithWorld ? NPC.AnyNPCs(ModContent.NPCType<Anahita>()) : NPC.AnyNPCs(ModContent.NPCType<Leviathan>())) || Main.LocalPlayer.Calamity().monolithLeviathanShader > 0;
    }
}

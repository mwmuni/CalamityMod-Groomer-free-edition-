using CalamityMod.NPCs.DevourerofGods;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class DoGBackgroundScene : BaseBiomeVisualSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        protected override string BiomeVisualKey => "CalamityMod:DevourerofGodsHead";

        public override bool IsSceneEffectActive(Player player)
        {
            bool monolithIsActive = Main.LocalPlayer.Calamity().monolithDevourerBShader > 0 || Main.LocalPlayer.Calamity().monolithDevourerPShader > 0;
            bool dogIsAlive = NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>());

            return dogIsAlive || monolithIsActive;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            base.SpecialVisuals(player, isActive);
            if (isActive)
                SkyManager.Instance.Activate("CalamityMod:DevourerofGodsHead");
            else
                SkyManager.Instance.Deactivate("CalamityMod:DevourerofGodsHead");
        }
    }
}

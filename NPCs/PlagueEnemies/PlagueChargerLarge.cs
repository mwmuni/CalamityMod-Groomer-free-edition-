using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class PlagueChargerLarge : PlagueChargerBase
    {
        public override LocalizedText DisplayName => CalamityUtils.GetText("NPCs.PlagueCharger.DisplayName");

        protected override int DamageValue => 45;
        protected override int DefenseValue => 36;
        protected override float ScaleMultiplier => 0.75f;
        protected override int LifeMaximum => 840;
        protected override float KnockBackResistance => 0.3f;
        protected override int ValueInSilver => 10;
        protected override int BannerType => ModContent.NPCType<PlagueCharger>();
        protected override int CanisterDropMinimum => 2;
        protected override int CanisterDropMaximum => 3;
        protected override int PlagueDebuffDuration => 180;
        protected override float DeathGoreScale => 0.75f;

        protected override Texture2D GetGlowTexture()
        {
            return ModContent.Request<Texture2D>("CalamityMod/NPCs/PlagueEnemies/PlagueChargerGlow").Value;
        }
    }
}

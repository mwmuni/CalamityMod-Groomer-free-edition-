using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlagueEnemies
{
    public class PlagueCharger : PlagueChargerBase
    {
        public static Asset<Texture2D> GlowTexture;

        protected override int DamageValue => 37;
        protected override int DefenseValue => 32;
        protected override float ScaleMultiplier => 0.5f;
        protected override int LifeMaximum => 720;
        protected override float KnockBackResistance => 0.4f;
        protected override int ValueInSilver => 5;
        protected override int BannerType => NPC.type;
        protected override int CanisterDropMinimum => 1;
        protected override int CanisterDropMaximum => 2;
        protected override int PlagueDebuffDuration => 120;
        protected override float DeathGoreScale => 0.5f;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        protected override Texture2D GetGlowTexture()
        {
            return GlowTexture.Value;
        }
    }
}

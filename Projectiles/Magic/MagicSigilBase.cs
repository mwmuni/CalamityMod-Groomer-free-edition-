using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public abstract class MagicSigilBase : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";

        protected bool HasSpawnedProjectile;

        protected virtual float FireTime => 35f;
        protected virtual float KillTime => 50f;
        protected virtual float MaskRevealTime => 24f;
        protected virtual float FadeStartTime => 35f;
        protected virtual float FadeEndTime => 50f;
        protected virtual bool CopiesParentScale => true;
        protected virtual bool IsTimeToFire => Projectile.localAI[0] == FireTime;

        protected abstract void FireSigilProjectile();

        protected virtual void DoAdditionalAI()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 74;
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile parent = Main.projectile[(int)Projectile.ai[0]];
            bool parentActive = parent != null && parent.active && parent.type == ModContent.ProjectileType<SigilSet>();

            if (!parentActive && Projectile.ai[2] == 0)
            {
                Projectile.Kill();
                return;
            }

            int i = (int)Projectile.ai[1];
            float dist = i % 3 == 0 ? 270f : 280f;
            float extraRot = i % 3 == 0 ? 0 : i % 3 == 1 ? MathHelper.ToRadians(-3.33f) : MathHelper.ToRadians(3.33f);

            Vector2 sigilPos = parent.Center + (Vector2.UnitX.RotatedBy(MathHelper.Lerp(0, MathHelper.TwoPi, i / 6f)) * dist).RotatedBy(parent.rotation + extraRot);
            Projectile.Center = sigilPos;

            if (Projectile.ai[2] > 0)
            {
                Projectile.localAI[0]++;

                if (IsTimeToFire && !HasSpawnedProjectile)
                    FireSigilProjectile();

                if (Projectile.localAI[0] >= KillTime)
                    Projectile.Kill();
            }
            else
            {
                if (CopiesParentScale)
                    Projectile.scale = parent.scale;
                Projectile.rotation = 0;
                Projectile.alpha = parent.alpha;
            }

            if (parentActive)
                Projectile.timeLeft = parent.timeLeft;

            DoAdditionalAI();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D mainTexture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Texture2D blankTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/BlankSigil").Value;

            float finalScale = Projectile.scale;
            float alphaOpacity = 1f - (Projectile.alpha / 255f);
            float maskOpacity = 0f;

            if (Projectile.ai[2] > 0)
            {
                float animationTime = Projectile.localAI[0];

                if (animationTime <= MaskRevealTime)
                    maskOpacity = Utils.GetLerpValue(0f, MaskRevealTime, animationTime, clamped: true);
                else
                    maskOpacity = 1f;

                if (animationTime >= FadeStartTime)
                {
                    float scaleFactor = Utils.GetLerpValue(FadeStartTime, FadeEndTime, animationTime, clamped: true);
                    finalScale = MathHelper.Lerp(Projectile.scale, 0f, scaleFactor);
                    alphaOpacity = MathHelper.Lerp(alphaOpacity, 0f, scaleFactor);
                }
            }

            Main.EntitySpriteDraw(mainTexture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * alphaOpacity, Projectile.rotation, mainTexture.Size() / 2, finalScale, SpriteEffects.None, 0);

            if (Projectile.ai[2] > 0)
                Main.EntitySpriteDraw(blankTexture, Projectile.Center - Main.screenPosition, null, Color.White * maskOpacity, Projectile.rotation, blankTexture.Size() / 2, finalScale, SpriteEffects.None, 0);

            return false;
        }
    }
}

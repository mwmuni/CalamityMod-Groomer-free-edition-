using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public abstract class LostSoulBase : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public ref float Time => ref Projectile.ai[0];

        protected abstract int ProjectileSize { get; }
        protected virtual float? ProjectileScale => null;
        protected virtual float TrailOffset => 0.8f;
        protected virtual float DeathDustPositionRadius => 42f;
        protected virtual float DeathDustVelocityRadius => 1.75f;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.SentryShot[Type] = true;
        }

        public override void SetDefaults()
        {
            if (ProjectileScale.HasValue)
                Projectile.scale = ProjectileScale.Value;
            Projectile.width = Projectile.height = ProjectileSize;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI() => LostSoulGiant.DoSoulAI(Projectile, ref Time);

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < Projectile.oldPos.Length / 2; j++)
                {
                    float fade = (float)Math.Pow(1f - Utils.GetLerpValue(0f, Projectile.oldPos.Length / 2, j, true), 2D);
                    Color drawColor = Color.Lerp(Projectile.GetAlpha(lightColor), Color.White * Projectile.Opacity, j / Projectile.oldPos.Length) * fade;
                    Vector2 drawPosition = Projectile.oldPos[j] + Projectile.Size * 0.5f + (MathHelper.TwoPi * i / 4f).ToRotationVector2() * TrailOffset - Main.screenPosition;
                    float rotation = Projectile.oldRot[j];

                    Main.EntitySpriteDraw(texture, drawPosition, frame, drawColor, rotation, frame.Size() * 0.5f, Projectile.scale, direction, 0);
                }
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // Play a wraith death sound.
            SoundEngine.PlaySound(SoundID.NPCDeath52, Projectile.Center);

            if (Main.dedServ)
                return;

            for (int i = 0; i < 45; i++)
            {
                Dust ectoplasm = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(DeathDustPositionRadius, DeathDustPositionRadius), DustID.PortalBoltTrail);
                ectoplasm.velocity = Main.rand.NextVector2Circular(DeathDustVelocityRadius, DeathDustVelocityRadius);
                ectoplasm.color = Projectile.GetAlpha(Color.White);
                ectoplasm.scale = 1.45f;
                ectoplasm.noGravity = true;
                ectoplasm.noLight = true;
            }
        }
    }
}

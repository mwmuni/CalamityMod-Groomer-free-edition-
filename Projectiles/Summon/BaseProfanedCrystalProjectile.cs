using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    // Abstract content bases keep the concrete projectile identities and texture paths.
    public abstract class BaseProfanedCrystalProjectile : ModProjectile
    {
        protected virtual Texture2D DrawTexture => Terraria.GameContent.TextureAssets.Projectile[Type].Value;

        public override bool PreAI()
        {
            Projectile.Calamity().overridesMinionDamagePrevention = true;
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return ProvUtils.GetColorBasedOnEnrage(!Main.dayTime, Projectile.alpha);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = DrawTexture;
            int num214 = texture.Height / Main.projFrames[Type];
            int y6 = num214 * Projectile.frame;
            Projectile.DrawBackglow(ProvUtils.GetColorBasedOnEnrage(!Main.dayTime, Projectile.alpha, true), 4f, texture);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, num214)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, num214 / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }

    public abstract class BaseProfanedCrystalFireball : BaseProfanedCrystalProjectile
    {
        protected abstract float FireballScale { get; }
        protected virtual bool DiesInWater => false;
        protected override Texture2D DrawTexture => Main.dayTime ? Terraria.GameContent.TextureAssets.Projectile[Type].Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyBlastNight").Value;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 180;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 75;
            Projectile.minion = true;
            Projectile.scale = FireballScale;
        }

        public override bool PreAI()
        {
            Projectile.Calamity().overridesMinionDamagePrevention = true;
            Projectile.LoopAnimation(6, 4, 0);
            if (Projectile.timeLeft == 50)
            {
                Projectile.tileCollide = true;
            }
            if (DiesInWater && Projectile.wet && !Projectile.lavaWet && Projectile.timeLeft < 70)
            {
                Projectile.Kill();
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            return false;
        }
    }

    public abstract class BaseProfanedCrystalFireballSplit : BaseProfanedCrystalProjectile
    {
        protected abstract int Lifetime { get; }
        protected abstract int HitCooldown { get; }
        protected override Texture2D DrawTexture => Main.dayTime ? Terraria.GameContent.TextureAssets.Projectile[Type].Value : ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/HolyFire2Night").Value;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.SummonTagDamageMultiplier[Type] = 0.3f;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = Lifetime;
            Projectile.minion = true;
            Projectile.gfxOffY = -25f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = HitCooldown;
            Projectile.DamageType = DamageClass.Summon;
        }
    }
}

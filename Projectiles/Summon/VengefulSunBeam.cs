using System;
using System.Linq;
using CalamityMod.Enums;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class VengefulSunBeam : BaseFireTrailProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        protected override float FireBodyWidth => 38f;
        protected override int FireCoreWidth => 24;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 14;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.MinionShot[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.MaxUpdates = 2;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0; //has a custom iframe counter
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
        }

        public override void AI()
        {
            if (Projectile.ai[0] > 1)
            {
                Projectile.ai[0]--;
                Projectile.timeLeft = 120;
            }
            if (Projectile.ai[0] > 0)
            {
                var target = Projectile.FindTargetWithinRange(640);
                if (target != null)
                {
                    Projectile.Calamity().HomingTarget = target.whoAmI;
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleLerp(Projectile.DirectionTo(target.Center).ToRotation(), 0.5f*(1-Projectile.ai[0]/120f)).ToRotationVector2() * Projectile.velocity.Length();
                }
            }
            if (Projectile.damage <= 0)
            {
                Projectile.ai[0] = 0;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4;
                Projectile.timeLeft = (int)MathHelper.Min(8, Projectile.timeLeft);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnFireHitDust(Color.OrangeRed, true);
            if (Projectile.ai[0] > 0)
                return;
            for (var i = -1; i <= 1; i+=2)
            {
                var randomVel = Projectile.velocity.RotatedByRandom(MathHelper.TwoPi);
                var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(),Projectile.Center- randomVel * 3, randomVel, Type,Projectile.damage,Projectile.knockBack,Projectile.owner, 120, Projectile.ai[1]);
                p.penetrate = 1;
                p.scale = 0.75f;
            }
            Projectile.damage = 0;
            Projectile.timeLeft = 10;
            Projectile.netUpdate = true;
        }

        public override bool? CanDamage()
        {
            return Projectile.ai[0] <= 90;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public Color FireColorFunction(float completion, Vector2 vertexPos)
        {
            Color mainColor = Color.Lerp(Color.Yellow, Color.OrangeRed, Projectile.ai[1]);
            if (Projectile.ai[1] >= 1)
                mainColor = Color.LightBlue;
            mainColor *= 1.3f;
            Color endColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            return Color.Lerp(mainColor, endColor, completion);
        }

        public Color FireCoreColorFunction(float completion, Vector2 vertexPos)
        {
            Color mainColor = Color.Black;
            mainColor = Color.Lerp(mainColor, Color.Gold, 1 - Projectile.ai[1]);
            Color tipColor = Color.Lerp(mainColor, Color.Transparent, Utils.GetLerpValue(0.8f, 1f, completion, true));
            Color fullBodyColor = Color.Lerp(mainColor, tipColor, completion);
            return fullBodyColor;
        }

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch, GeneralDrawLayer layer)
        {
            DrawPixelatedFireTrail(FireColorFunction, FireCoreColorFunction);
        }
    }
}

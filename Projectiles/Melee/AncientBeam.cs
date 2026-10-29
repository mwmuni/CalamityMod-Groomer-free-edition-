using System;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;


namespace CalamityMod.Projectiles.Melee
{
    public class AncientBeam : BaseEnergyNeedleProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Melee/FracturedArkGlow";

        const float MaxTime = 40;
        public float Timer => MaxTime - Projectile.timeLeft;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = (int)MaxTime;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
        }

        protected override float CollisionBladeLength => 34f;
        protected override float CollisionStartOffset => 8.5f;

        public override void AI()
        {
            if (Projectile.timeLeft < MaxTime - 5)
                Projectile.tileCollide = true;


            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Projectile.scale = 2.4f;
            Projectile.Opacity = 0.6f;
            Lighting.AddLight(Projectile.Center, 0.75f, 1f, 0.24f);

            Projectile.velocity *= (1 - (float)Math.Pow(Timer / MaxTime, 3));

            if (Main.rand.NextBool(3))
            {
                int dustTrail = Dust.NewDust(Projectile.Center, 14, 14, DustID.RainbowTorch, Projectile.velocity.X * 0.05f, Projectile.velocity.Y * 0.05f, 150, new Color(Main.DiscoR, 100, 255), 1.2f);
                Main.dust[dustTrail].noGravity = true;
            }

            if (Main.rand.NextBool(3))
            {
                int dustType = Main.rand.Next(3);
                dustType = dustType == 0 ? 15 : dustType == 1 ? 57 : 58;

                Dust.NewDust(Projectile.Center, 14, 14, dustType, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default, 1.3f);
            }

            if (Projectile.velocity.Length() < 1.0f)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 35)
                return false;

            DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}

using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TerrorBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 4;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
            Projectile.light = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        private void SpawnTerrorBlast()
        {
            int projID = ModContent.ProjectileType<TerrorBlast>();
            int blastDamage = (int)(Projectile.damage * TerrorBlade.TerrorBlastMultiplier);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, projID, blastDamage, Projectile.knockBack, Projectile.owner);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                if (Projectile.owner == Main.myPlayer)
                {
                    SpawnTerrorBlast();
                }
                Projectile.BounceOnTileCollision(oldVelocity);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                SpawnTerrorBlast();
            }
        }

        public override void AI()
        {
            if (Projectile.localAI[1] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item60, Projectile.position);
                Projectile.localAI[1] += 1f;
            }
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 595)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 2);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            // If no on-hit explosion was ever generated, spawn it for free when the beam expires.
            if (Projectile.localAI[0] == 0f)
                SpawnTerrorBlast();
        }
    }
}

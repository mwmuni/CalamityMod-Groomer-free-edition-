using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Healing;

namespace CalamityMod.Projectiles.Rogue
{
    public class BloodsoakedCrashax : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BloodsoakedCrasher";

        private int bounce = 3; //number of times it bounces
        private int grind = 0; //used to know when to slow down
        private const float MaxSpeed = 14f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 600; //10 seconds and counting (but not actually because extra updates)
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            float speed = Projectile.velocity.Length();
            if (grind > 0)
            {
                grind--;
                // Suddenly stop when on top of enemies.
                Projectile.velocity.X *= 0.75f;
                Projectile.velocity.Y *= 0.75f;
            }
            else
            {
                // Gravity
                Projectile.velocity.Y += 0.07f;

                // Cap velocity.
                speed = Projectile.velocity.Length();
                if (speed > MaxSpeed)
                    Projectile.velocity *= MaxSpeed / speed;
            }

            // Spin constantly, but even faster when grinding or going fast
            float spinRate = grind > 0 ? 0.28f : 0.09f;
            if (grind <= 0)
                spinRate += speed * 0.005f;
            Projectile.rotation += spinRate * Projectile.direction;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
            {
                Projectile.Kill(); //you can only bounce so much 'til death
            }
            else
            {
                Projectile.BounceOnTileCollision(oldVelocity);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Laceration>(), 120);
            if (target.lifeMax > 5)
                OnHitEffects(hit.Damage);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<Laceration>(), 120);
            OnHitEffects(info.Damage);
        }

        private void OnHitEffects(int damage)
        {
            grind += 6; //THE GRIND NEVER STOPS
            if (grind > 18)
                grind = 18; // except when it's too much

            if (Projectile.Calamity().stealthStrike && Projectile.owner == Main.myPlayer) //stealth strike attack
            {
                int projID = ModContent.ProjectileType<Blood>();
                int stealth = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.UnitX.RotatedByRandom(MathHelper.Pi) * 4f, projID, (int)(Projectile.damage * 0.5f), 1f, Projectile.owner, 1f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (stealth.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[stealth].DamageType = RogueDamageClass.Instance;
                    Main.projectile[stealth].extraUpdates = 1;
                }
            }

            float orbAmount = Projectile.Calamity().stealthStrike ? 3 : Projectile.penetrate % 2;
            if (orbAmount > 0)
            {
                float spreadAmount = MathHelper.ToRadians(360);
                for (var i = 0; i < orbAmount; i++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.One.RotatedByRandom(spreadAmount) * 2f * Main.rand.NextFloat(0.75f, 1.25f), ModContent.ProjectileType<BloodstoneHealOrb>(), 10, 0f, Projectile.owner);

                }
            }
        }

        public override bool PreDraw(ref Color lightColor) //afterimages
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}

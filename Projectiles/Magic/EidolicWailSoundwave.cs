using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.NPCs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    [PierceResistException]
    public class EidolicWailSoundwave : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.scale = 0.005f;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 11;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
            Projectile.timeLeft = 450;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] < 1f)
            {
                Projectile.localAI[0] += 0.05f; // 20 frames to reach full size
                Projectile.scale += 0.05f;
                Projectile.width = (int)(36f * Projectile.scale);
                Projectile.height = (int)(36f * Projectile.scale);
            }
            else
            {
                Projectile.width = 36;
                Projectile.height = 36;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.BounceOnTileCollision(oldVelocity);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.75f); // 25% damage nerf for every enemy hit
            if (Projectile.damage < 1)
                Projectile.damage = 1;

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HadopelagicPressure>(), 240);
            if (Projectile.numHits == 0)
                Projectile.velocity *= 0.4f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
    }
}

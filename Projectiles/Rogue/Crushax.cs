using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class Crushax : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";

        public override void SetStaticDefaults() => ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.03f * Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 20f)
            {
                if (CalamityUtils.ClosestNPCAt(Projectile.Center, 280f) == null)
                {
                    Projectile.velocity.Y += 0.35f;
                    Projectile.velocity.X *= 0.97f;
                }
                else
                    CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 280f, 14f, 20f);
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (Projectile.velocity.Y > 15f)
                Projectile.velocity.Y = 15f;
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
                Projectile.BounceOnTileCollision(oldVelocity);
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 180);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<HeavyBleeding>(), 180);
    }
}

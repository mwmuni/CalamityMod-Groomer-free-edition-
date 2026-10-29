using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class BouncingShotgunPellet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Ranged/RealmRavagerBullet";
        private int bounce = 3;
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            AIType = ProjectileID.BulletHighVelocity;
            Projectile.timeLeft = 180;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
                Projectile.Kill();

            Player owner = Main.player[Projectile.owner];

            if (bounce == 1)
            {
                float npcDistCheck = 640f; // 40 tiles
                int index = -1;
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (!n.CanBeChasedBy(Projectile))
                        continue;

                    float currentNPCDist = Vector2.Distance(n.Center, owner.ClampedMouseWorld());
                    if (currentNPCDist < npcDistCheck)
                    {
                        npcDistCheck = currentNPCDist;
                        index = n.whoAmI;
                    }
                }
                // If the index is not default, smart bounce in the direction of that enemy.
                if (index != -1)
                {
                    Projectile.velocity = CalamityUtils.CalculatePredictiveAimToTargetMaxUpdates(Projectile.Center, Main.npc[index], owner.HeldItem.shootSpeed, 3);
                }
                return false;
            }
            else // Otherwise, use standard bouncing behavior.
            {
                Projectile.BounceOnTileCollision(oldVelocity);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(32);
            for (int d = 0; d < 2; d++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
            }
            for (int d = 0; d < 20; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 0, default, 2.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default, 1.5f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
        }
    }
}

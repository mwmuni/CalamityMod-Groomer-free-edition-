using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class ForbiddenCircletEater : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        private int bounce = 3;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.MinionShot[Type] = true; //This marks it as a minion shot, which allows it to proc summon tag effects.
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 3;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 50;
            }
            else
            {
                Projectile.extraUpdates = 0;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= 4)
            {
                Projectile.frame = 0;
            }
            int dustType = 159;
            float slowXVel = Projectile.velocity.X / 3f;
            float slowYVel = Projectile.velocity.Y / 3f;
            int dustID = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, 1f);
            Dust dust = Main.dust[dustID];
            dust.position.X = Projectile.Center.X - slowXVel;
            dust.position.Y = Projectile.Center.Y - slowYVel;
            dust.velocity *= 0f;
            dust.scale = 0.5f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            float projX = Projectile.position.X;
            float projY = Projectile.position.Y;
            float attackDistance = 100000f;
            Projectile.ai[0] += 1f;

            NPC target = null;
            if (Projectile.ai[0] > 30f)
            {
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float npcX = npc.position.X + (float)(npc.width / 2);
                        float npcY = npc.position.Y + (float)(npc.height / 2);
                        float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                        if (npcDist < 640f && npcDist < attackDistance && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                        {
                            target = npc;
                        }
                    }
                }
                if (target.IsNullOrInactive())
                {
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.CanBeChasedBy(Projectile, false))
                        {
                            float npcX = npc.position.X + (float)(npc.width / 2);
                            float npcY = npc.position.Y + (float)(npc.height / 2);
                            float npcDist = Math.Abs(Projectile.position.X + (float)(Projectile.width / 2) - npcX) + Math.Abs(Projectile.position.Y + (float)(Projectile.height / 2) - npcY);
                            if (npcDist < 640f && npcDist < attackDistance && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height))
                            {

                                target = npc;
                            }
                        }
                    }
                }
            }
            if (!target.IsNullOrInactive())
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target.Center) * 15, 0.1f);
            else
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.velocity.SafeNormalize(Vector2.Zero) * 15, 0.1f);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bounce--;
            if (bounce <= 0)
            {
                Projectile.Kill();
            }
            else
            {
                Projectile.ai[0] += 15f;
                Projectile.BounceOnTileCollision(oldVelocity);
            }
            return false;
        }

        public override bool? CanDamage() => Projectile.ai[0] >= 30f;
    }
}

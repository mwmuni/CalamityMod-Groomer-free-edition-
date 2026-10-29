using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{
    public abstract class BaseHomingCoinDaggerProjectile : ModProjectile, ILocalizedModType
    {
        protected abstract int CoinDustType { get; }

        public new string LocalizationCategory => "Projectiles.Rogue";

        private bool hasHitEnemy = false;
        private int targetNPC = -1;
        private List<int> previousNPCs = new List<int>() { -1 };

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Main.rand.NextBool(7))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, CoinDustType, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }
            if (hasHitEnemy)
            {
                Projectile.rotation += Projectile.direction * 0.4f;
            }
            else
            {
                Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
                Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.PiOver2);
                //Rotating 90 degrees if shooting right
                if (Projectile.spriteDirection == 1)
                {
                    Projectile.rotation += MathHelper.ToRadians(90f);
                }
            }

            if (!Projectile.Calamity().stealthStrike && Projectile.timeLeft < 575)
            {
                Projectile.velocity.Y += 0.5f;
                if (Projectile.velocity.Y > 16f)
                {
                    Projectile.velocity.Y = 16f;
                }
            }
            else if (Projectile.Calamity().stealthStrike && hasHitEnemy)
            {
                if (targetNPC >= 0 && Main.npc[targetNPC].active)
                {
                    Vector2 newVelocity = Main.npc[targetNPC].Center - Projectile.Center;
                    newVelocity.Normalize();
                    newVelocity *= 15f;
                    Projectile.velocity = newVelocity;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float minDist = 999f;
            int index = 0;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                bool hasHitNPC = false;
                for (int j = 0; j < previousNPCs.Count; j++)
                {
                    if (previousNPCs[j] == npc.whoAmI)
                    {
                        hasHitNPC = true;
                    }
                }

                if (npc == target)
                {
                    previousNPCs.Add(npc.whoAmI);
                }
                if (npc.CanBeChasedBy(Projectile, false) && npc != target && !hasHitNPC)
                {
                    float dist = (Projectile.Center - npc.Center).Length();
                    if (dist < minDist)
                    {
                        minDist = dist;
                        index = npc.whoAmI;
                    }
                }
            }

            Vector2 velocityNew;
            if (minDist < 999f)
            {
                if (Projectile.Calamity().stealthStrike)
                {
                    Projectile.damage = (int)(Projectile.damage * 1.05f);
                }
                hasHitEnemy = true;
                targetNPC = index;
                velocityNew = Main.npc[index].Center - Projectile.Center;
                velocityNew.Normalize();
                velocityNew *= 15f;
                Projectile.velocity = velocityNew;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int num621 = 0; num621 < 8; num621++)
            {
                int num622 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, CoinDustType, 0f, 0f, 100, default, 1f);
                Main.dust[num622].velocity *= 1f;
            }
        }
    }
}

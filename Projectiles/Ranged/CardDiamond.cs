using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class CardDiamond : BaseCardProjectile
    {
        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 50;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int d = 0; d < 10; d++)
            {
                int paper = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Web, 0f, 0f, 100, default, 2f);
                Main.dust[paper].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[paper].scale = 0.5f;
                    Main.dust[paper].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 15; d++)
            {
                int paper = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Web, 0f, 0f, 100, default, 3f);
                Main.dust[paper].noGravity = true;
                Main.dust[paper].velocity *= 5f;
                paper = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Web, 0f, 0f, 100, default, 2f);
                Main.dust[paper].velocity *= 2f;
            }

            if (!Main.dedServ)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < (goreAmt / 3))
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= (2 * goreAmt / 3))
                    {
                        velocityMult = 1f;
                    }
                    Mod mod = ModContent.GetInstance<CalamityMod>();
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }
        }

    }
}

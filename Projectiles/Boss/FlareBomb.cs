using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class FlareBomb : BaseFlareExplosionProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 66;
            Projectile.hostile = true;
            Projectile.scale = 1.5f;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.alpha = 50;
            Projectile.timeLeft = 180;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Type])
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0.5f, 0.25f, 0f);

            if (Projectile.timeLeft > 30 && Projectile.alpha > 0)
                Projectile.alpha -= 25;
            if (Projectile.timeLeft > 30 && Projectile.alpha < 128 && Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                Projectile.alpha = 128;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            if (Projectile.ai[0] == -1f || (Projectile.timeLeft > 135 && Projectile.ai[1] == 1f))
                return;

            float inertia = revenge ? 70f : 77f;
            float velocityMult = revenge ? 35f : 28f;
            int playerTracker = (int)Projectile.ai[0];
            if (playerTracker >= 0 && Main.player[playerTracker].active && !Main.player[playerTracker].dead)
            {
                if (Projectile.Distance(Main.player[playerTracker].Center) > 40f)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[playerTracker].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + moveDirection * velocityMult) / inertia;
                }
            }
            else
            {
                if (Projectile.ai[0] != -1f)
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }
            }

            if (Projectile.timeLeft < 60)
                return;

            float acceleration = 0.5f;
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.whoAmI != Projectile.whoAmI && p.type == Projectile.type)
                {
                    if (Vector2.Distance(Projectile.Center, p.Center) < 24f)
                    {
                        if (Projectile.position.X < p.position.X)
                            Projectile.velocity.X -= acceleration;
                        else
                            Projectile.velocity.X += acceleration;
                        
                        if (Projectile.position.Y < p.position.Y)
                            Projectile.velocity.Y -= acceleration;
                        else
                            Projectile.velocity.Y += acceleration;
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            int framing = texture.Height / Main.projFrames[Type];
            int y6 = framing * Projectile.frame;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture.Width, framing), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 16f * Projectile.scale, targetHitbox);

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<Dragonfire>(), 60);
        }
    }
}

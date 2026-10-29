using System;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Yoyos
{
    public class SulphurousGrabberYoyo : BaseYoyoProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<SulphurousGrabber>();
        private int bubbleCounter = 0;
        private bool bubbleStronk = false;
        private int bubbleStronkCounter = 0;
        private float arbitraryTimer = 0f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Type] = -1f;
            ProjectileID.Sets.YoyosMaximumRange[Type] = SulphurousGrabber.Reach;
            ProjectileID.Sets.YoyosTopSpeed[Type] = SulphurousGrabber.Speed / 2;

            ProjectileID.Sets.TrailCacheLength[Type] = 4;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 18;
            // Hit cooldown set in AI
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                if (bubbleStronk)
                {
                    Projectile.MaxUpdates = 3;
                    Projectile.localNPCHitCooldown = 10 * Projectile.MaxUpdates;
                    bubbleStronkCounter++;
                }
                else
                {
                    Projectile.MaxUpdates = 2;
                    Projectile.localNPCHitCooldown = 12 * Projectile.MaxUpdates;
                    bubbleStronkCounter = 0;
                }

                if (bubbleStronkCounter >= 180)
                    bubbleStronk = false;

                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.type == ModContent.ProjectileType<SulphurousGrabberBubble2>() && proj.ai[0] >= 40f && proj.owner == Projectile.owner)
                    {
                        if (Projectile.Hitbox.Intersects(proj.Hitbox))
                        {
                            proj.Kill();
                            bubbleStronk = true;
                            bubbleStronkCounter = 0;
                            break;
                        }
                    }
                }

                arbitraryTimer += bubbleStronk ? 0.5f : 1f;

                bubbleCounter++;
                if (bubbleCounter >= 60)
                {
                    int bubbleAmt = 3;
                    for (float i = 0; i < bubbleAmt; i++)
                    {
                        int projType = ModContent.ProjectileType<SulphurousGrabberBubble>();
                        if (Main.rand.NextBool(8))
                            projType = ModContent.ProjectileType<SulphurousGrabberBubble2>();
                        float angle = MathHelper.TwoPi / bubbleAmt * i + (float)Math.Sin(arbitraryTimer / 20f) * MathHelper.PiOver2;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, angle.ToRotationVector2() * 10f, projType, Projectile.damage / 2, Projectile.knockBack / 4, Projectile.owner);
                    }
                    bubbleCounter = 0;
                }
            }

            if ((Projectile.position - Main.player[Projectile.owner].position).Length() > 3200f) //200 blocks
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            if (bubbleStronk)
            {
                tex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/Yoyos/SulphurousGrabberYoyoBubble").Value;
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1, tex);
            }
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<Irradiated>(), 120);
    }
}

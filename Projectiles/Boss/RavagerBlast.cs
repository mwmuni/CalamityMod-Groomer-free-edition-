using System.IO;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class RavagerBlast : BaseLaserbeamProjectile, ILocalizedModType
    {
        //Modified clone of Seraphim's Laser
        public new string LocalizationCategory => "Projectiles.Boss";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int OwnerIndex
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public bool blue => Projectile.ai[1] < 0f;

        public override float MaxScale => Projectile.ai[0];
        public override float MaxLaserLength => blue ? 4800f : 2400f;
        public override float Lifetime => blue ? 200 : 50;
        public override Color LaserOverlayColor
        {
            get
            {
                Color c1 = blue ? Color.MediumTurquoise : (CalamityClientConfig.Instance.Photosensitivity ? new Color(79, 79, 79) : Color.Gray);
                Color c2 = blue ? Color.Cyan : (CalamityClientConfig.Instance.Photosensitivity ? new Color(79, 79, 79) : Color.White);
                Color color = Color.Lerp(c1, c2, Projectile.identity % 5f / 5f) * 1.1f;
                color.A = 0;
                return color;
            }
        }

        public override Color LightCastColor => Color.Transparent;
        public override Texture2D LaserBeginTexture => ModContent.Request<Texture2D>("CalamityMod/Projectiles/Rogue/SeraphimBeamLarge", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserMiddleTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/SeraphimBeamLargeMiddle", AssetRequestMode.ImmediateLoad).Value;
        public override Texture2D LaserEndTexture => ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Lasers/SeraphimBeamLargeEnd", AssetRequestMode.ImmediateLoad).Value;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 10;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 200;
            Projectile.Calamity().DealsDefenseDamage = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.Write(Projectile.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            Projectile.scale = reader.ReadSingle();
        }

        public override void AttachToSomething() { }

        public override void UpdateLaserMotion()
        {
            // Update the direction and rotation of the laser.
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (blue)
            {
                float speed = 8f + 8f * Utils.GetLerpValue(Lifetime, 0f, Projectile.timeLeft);
                switch (OwnerIndex)
                {
                    case -1: //go right
                        Projectile.position.X += speed;
                        break;
                    case -2: //go left
                        Projectile.position.X -= speed;
                        break;
                    case -3: //go down
                        Projectile.position.Y += speed;
                        break;
                    default: //go up
                        Projectile.position.Y -= speed;
                        break;
                }
            }
        }

        public override float DetermineLaserLength() => MaxLaserLength;

        public override void PostAI()
        {
            // Determine frames.
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5f == 4f)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
        }

        // Can hit if white (regardless of condition) or blue only if the player is (close to) still
        public override bool CanHitPlayer(Player target) => (!blue || target.velocity.Length() >= 0.25f) && Projectile.scale >= 0.5f;

        public override bool PreDraw(ref Color lightColor)
        {
            return global::CalamityMod.Projectiles.LaserBeamRendering.DrawSeraphimStyleBeam(this);
        }
    }
}

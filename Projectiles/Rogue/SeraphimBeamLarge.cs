using System;
using System.IO;
using System.Linq;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SeraphimBeamLarge : BaseLaserbeamProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int OwnerIndex
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override float MaxScale => 1f;
        public override float MaxLaserLength => 1000f;
        public override float Lifetime => 50;
        public override Color LaserOverlayColor
        {
            get
            {
                Color c1 = Color.Goldenrod;
                Color c2 = Color.Orange;
                Color color = Color.Lerp(c1, c2, Projectile.identity % 5f / 5f) * 1.1f;
                color.A = 25;
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
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = RogueDamageClass.Instance;
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
        }

        public override float DetermineLaserLength()
        {
            float[] sampledLengths = new float[10];
            Collision.LaserScan(Projectile.Center, Projectile.velocity, Projectile.width * Projectile.scale, MaxLaserLength, sampledLengths);
            float newLaserLength = sampledLengths.Average();

            if (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
                return MaxLaserLength;

            return newLaserLength;
        }

        public override void PostAI()
        {
            if (Projectile.frameCounter == 0)
                SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound, Projectile.Center);

            // Determine frames.
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5f == 4f)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
        }

        // Rapidly decrease damage every hit
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => Projectile.damage = Math.Max(1, (int)(Projectile.damage * 0.3));

        public override bool PreDraw(ref Color lightColor)
        {
            return global::CalamityMod.Projectiles.LaserBeamRendering.DrawSeraphimStyleBeam(this);
        }
    }
}

using System.IO;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class PermafrostBlast : BaseLaserbeamProjectile, ILocalizedModType
    {
        // Modified clone of Seraphim's Laser
        public new string LocalizationCategory => "Projectiles.Boss";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public int OwnerIndex
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override float MaxScale => Projectile.ai[0];

        public override float MaxLaserLength => 2400f;

        public override float Lifetime => 50;

        public override Color LaserOverlayColor
        {
            get
            {
                return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
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
            CooldownSlot = ImmunityCooldownID.Bosses;
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

        public override void UpdateLaserMotion()
        {
            // Update the direction and rotation of the laser.
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override float DetermineLaserLength() => MaxLaserLength;

        public override void PostAI()
        {
            // Determine frames.
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 5f == 4f)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
        }

        public override bool CanHitPlayer(Player target) => Projectile.scale >= 0.5f;

        public override bool PreDraw(ref Color lightColor)
        {
            return global::CalamityMod.Projectiles.LaserBeamRendering.DrawSeraphimStyleBeam(this);
        }
    }
}

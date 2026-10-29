using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace CalamityMod.Projectiles.Boss
{
    public abstract class BasePhantomBlastProjectile : BasePhantomProjectile
    {
        protected override int ProjectileSize => 18;

        protected abstract int BlastDustType { get; }
        protected abstract float MinimumVelocity { get; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            Projectile.localAI[1] = reader.ReadSingle();
        }

        public override void AI()
        {
            PlayInitialSoundAndUpdateRotation();

            if (Projectile.velocity.Length() < MinimumVelocity)
                Projectile.velocity *= 1.01f;

            UpdateAlphaFade();

            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] == 24f)
            {
                Projectile.localAI[1] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 dustRotation = Vector2.UnitX * -Projectile.width / 2f;
                    dustRotation += -Vector2.UnitY.RotatedBy(l * MathHelper.Pi / 6f) * new Vector2(8f, 16f);
                    dustRotation = dustRotation.RotatedBy(Projectile.rotation - MathHelper.PiOver2);
                    int phantomDust = Dust.NewDust(Projectile.Center, 0, 0, BlastDustType, 0f, 0f, 160, default, 1f);
                    Main.dust[phantomDust].scale = 1.1f;
                    Main.dust[phantomDust].noGravity = true;
                    Main.dust[phantomDust].position = Projectile.Center + dustRotation;
                    Main.dust[phantomDust].velocity = Projectile.velocity * 0.1f;
                    Main.dust[phantomDust].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[phantomDust].position) * 1.25f;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item125, Projectile.Center);
            Projectile.position.X = Projectile.position.X + (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (Projectile.height / 2);
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.position.X = Projectile.position.X - (Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (Projectile.height / 2);
            for (int i = 0; i < 3; i++)
            {
                int killGhostDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, BlastDustType, 0f, 0f, 100, default, 1.2f);
                Main.dust[killGhostDust].velocity *= 3f;
                Main.dust[killGhostDust].noGravity = true;
                if (Main.rand.NextBool())
                {
                    Main.dust[killGhostDust].scale = 0.5f;
                    Main.dust[killGhostDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 5; j++)
            {
                int killGhostDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, BlastDustType, 0f, 0f, 100, default, 1.7f);
                Main.dust[killGhostDust2].noGravity = true;
                Main.dust[killGhostDust2].velocity *= 5f;
                killGhostDust2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, BlastDustType, 0f, 0f, 100, default, 1f);
                Main.dust[killGhostDust2].velocity *= 2f;
            }
        }
    }
}

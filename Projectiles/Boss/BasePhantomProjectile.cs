using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public abstract class BasePhantomProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";

        protected abstract int ProjectileSize { get; }
        protected abstract Color ProjectileColor { get; }

        public override void SetDefaults()
        {
            Projectile.width = ProjectileSize;
            Projectile.height = ProjectileSize;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        protected void PlayInitialSoundAndUpdateRotation()
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            }

            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;
        }

        protected void UpdateAlphaFade()
        {
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 9f)
            {
                Projectile.alpha -= 5;
                if (Projectile.alpha < 30)
                    Projectile.alpha = 30;
            }
        }

        public override Color? GetAlpha(Color lightColor) => ProjectileColor;
    }
}

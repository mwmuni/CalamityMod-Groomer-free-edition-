using CalamityMod.DataStructures;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class TerratomereExplosion : ModProjectile, IAdditiveDrawer, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 520;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.MaxUpdates * 14;
            Projectile.scale = 0.2f;
            Projectile.hide = true;
        }

        public override void AI() => UpdateExplosion(Projectile, true);

        internal static void UpdateExplosion(Projectile projectile, bool terra)
        {
            // Play an explosion sound on the first frame of this projectile's existence.
            if (projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SubsumingVortex.ExplosionSound with { Volume = 0.6f }, projectile.Center);
                projectile.localAI[0] = 1f;
            }

            // Emit a strong white light.
            Lighting.AddLight(projectile.Center, Color.White.ToVector3() * 1.5f);

            // Determine frames. Once the maximum frame is reached the projectile dies.
            projectile.frameCounter++;
            if (projectile.frameCounter % 5 == 4)
                projectile.frame++;
            if (projectile.frame >= 17)
                projectile.Kill();

            // Exponentially accelerate.
            projectile.scale *= terra ? Terratomere.ExplosionExpandFactor : 1.013f;
            projectile.Opacity = Utils.GetLerpValue(5f, 36f, projectile.timeLeft, true);
        }

        public void AdditiveDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/Skies/XerocLight").Value;
            Rectangle frame = texture.Frame(3, 6, Projectile.frame / 6, Projectile.frame % 6);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = frame.Size() * 0.5f;

            for (int i = 0; i < 2; i++)
            {
                Vector2 lightDrawPosition = drawPosition + (MathHelper.TwoPi * i / 36f + Main.GlobalTimeWrappedHourly * 5f).ToRotationVector2() * Projectile.scale * 12f;
                Color lightBurstColor = CalamityUtils.MulticolorLerp(Projectile.timeLeft / 144f, Terratomere.TerraColor1, Terratomere.TerraColor2);
                lightBurstColor = Color.Lerp(lightBurstColor, Color.White, 0.4f) * Projectile.Opacity * 0.24f;
                Main.spriteBatch.Draw(lightTexture, lightDrawPosition, null, lightBurstColor, 0f, lightTexture.Size() * 0.5f, Projectile.scale * 1.32f, SpriteEffects.None, 0);
            }
            if (Projectile.timeLeft < 149)
                Main.spriteBatch.Draw(texture, drawPosition, frame, Color.White, 0f, origin, 1.6f, SpriteEffects.None, 0);
        }
    }
}

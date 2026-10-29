using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace CalamityMod.Projectiles
{
    internal static class LaserBeamRendering
    {
        internal static bool DrawSeraphimStyleBeam(BaseLaserbeamProjectile beam)
        {
            if (beam.Projectile.velocity == Vector2.Zero)
                return false;

            Color beamColor = beam.LaserOverlayColor;
            Rectangle startFrameArea = beam.LaserBeginTexture.Frame(1, Main.projFrames[beam.Type], 0, beam.Projectile.frame);
            Rectangle middleFrameArea = beam.LaserMiddleTexture.Frame(1, Main.projFrames[beam.Type], 0, beam.Projectile.frame);
            Rectangle endFrameArea = beam.LaserEndTexture.Frame(1, Main.projFrames[beam.Type], 0, beam.Projectile.frame);

            Vector2 centerOnLaser = beam.Projectile.Center + beam.Projectile.velocity * beam.Projectile.scale * 116f;
            Main.EntitySpriteDraw(beam.LaserBeginTexture,
                             centerOnLaser - Main.screenPosition,
                             startFrameArea,
                             beamColor,
                             beam.Projectile.rotation,
                             beam.LaserBeginTexture.Size() / 2f,
                             beam.Projectile.scale,
                             SpriteEffects.None,
                             0);

            float laserBodyLength = beam.LaserLength + middleFrameArea.Height;

            if (laserBodyLength > 0f)
            {
                float laserOffset = middleFrameArea.Height * beam.Projectile.scale;
                float incrementalBodyLength = 0f;
                while (incrementalBodyLength + 1f < laserBodyLength)
                {
                    centerOnLaser += beam.Projectile.velocity * laserOffset;
                    incrementalBodyLength += laserOffset;
                    Main.EntitySpriteDraw(beam.LaserMiddleTexture,
                                     centerOnLaser - Main.screenPosition,
                                     middleFrameArea,
                                     beamColor,
                                     beam.Projectile.rotation,
                                     beam.LaserMiddleTexture.Size() * 0.5f,
                                     beam.Projectile.scale,
                                     SpriteEffects.None,
                                     0);
                }
            }
            Vector2 laserEndCenter = centerOnLaser + beam.Projectile.velocity * endFrameArea.Height * beam.Projectile.scale - Main.screenPosition;
            Main.EntitySpriteDraw(beam.LaserEndTexture,
                             laserEndCenter,
                             endFrameArea,
                             beamColor,
                             beam.Projectile.rotation,
                             beam.LaserEndTexture.Size() * 0.5f,
                             beam.Projectile.scale,
                             SpriteEffects.None,
                             0);
            return false;
        }
    }
}

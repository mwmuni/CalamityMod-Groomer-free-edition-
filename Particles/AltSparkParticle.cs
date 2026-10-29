using Microsoft.Xna.Framework;

namespace CalamityMod.Particles
{
    public class AltSparkParticle : GravityFadingParticle
    {
        public override bool UseAdditiveBlend => false;
        public override string Texture => "CalamityMod/Projectiles/StarProj";

        public AltSparkParticle(Vector2 relativePosition, Vector2 velocity, bool affectedByGravity, int lifetime, float scale, Color color)
            : base(relativePosition, velocity, affectedByGravity, lifetime, scale, color)
        {
        }
    }
}

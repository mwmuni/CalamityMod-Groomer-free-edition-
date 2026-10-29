using Microsoft.Xna.Framework;

namespace CalamityMod.Particles
{
    public class LineParticle : GravityFadingParticle
    {
        public override bool UseAdditiveBlend => true;
        public override string Texture => "CalamityMod/Particles/DrainLineBloom";

        public LineParticle(Vector2 relativePosition, Vector2 velocity, bool affectedByGravity, int lifetime, float scale, Color color)
            : base(relativePosition, velocity, affectedByGravity, lifetime, scale, color)
        {
        }
    }
}

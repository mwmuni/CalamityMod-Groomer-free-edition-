using Microsoft.Xna.Framework;

namespace CalamityMod.Particles
{
    public class SquareParticle : GravityFadingParticle
    {
        public override bool UseAdditiveBlend => true;
        protected override Vector2 DrawScale => Vector2.One;
        protected override bool DrawInnerLayer => false;
        protected override bool IncludeExtraRotation => true;
        protected override bool UseScalarDrawScale => true;

        public override string Texture => "CalamityMod/Particles/Square";

        public SquareParticle(Vector2 relativePosition, Vector2 velocity, bool affectedByGravity, int lifetime, float scale, Color color, float extraRotation = 0f)
            : base(relativePosition, velocity, affectedByGravity, lifetime, scale, color, extraRotation)
        {
        }
    }
}

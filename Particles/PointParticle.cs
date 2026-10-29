using Microsoft.Xna.Framework;

namespace CalamityMod.Particles
{
    public class PointParticle : GravityFadingParticle
    {
        public bool UseAltVisual = true;
        public override bool UseAdditiveBlend => UseAltVisual;
        protected override bool ApplyLightingDuringUpdate => true;
        public override string Texture => "CalamityMod/Particles/PointParticle";

        public PointParticle(Vector2 relativePosition, Vector2 velocity, bool affectedByGravity, int lifetime, float scale, Color color, bool AddativeBlend = true, bool affectedByLight = false)
            : base(relativePosition, velocity, affectedByGravity, lifetime, scale, color)
        {
            UseAltVisual = AddativeBlend;
            AffectedByLight = affectedByLight;
        }
    }
}

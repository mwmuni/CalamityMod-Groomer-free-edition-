using Microsoft.Xna.Framework;

namespace CalamityMod.Particles
{
    public class WaterFlavoredParticle : GravityFadingParticle
    {
        public override bool UseAdditiveBlend => false;
        protected override float ScaleDecay => 0.93f;
        protected override Vector2 DrawScale => new Vector2(0.65f, 1f);
        protected override bool ApplyLightingDuringDraw => true;

        public override string Texture => "CalamityMod/Particles/WaterFlavored";

        public WaterFlavoredParticle(Vector2 relativePosition, Vector2 velocity, bool affectedByGravity, int lifetime, float scale, Color color)
            : base(relativePosition, velocity, affectedByGravity, lifetime, scale, color)
        {
        }
    }
}

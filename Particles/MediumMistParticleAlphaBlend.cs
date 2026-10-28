using Microsoft.Xna.Framework;
using Terraria;

namespace CalamityMod.Particles
{
    public class MediumMistParticleAlphaBlend : Particle
    {
        public override string Texture => "CalamityMod/Particles/MediumSmoke";
        public override int FrameVariants => 3;

        private float Opacity;
        private Color ColorFire;
        private Color ColorFade;
        private float Spin;

        public MediumMistParticleAlphaBlend(Vector2 position, Vector2 velocity, Color colorFire, Color colorFade, float scale, float opacity, float rotationSpeed = 0f)
        {
            Position = position;
            Velocity = velocity;
            ColorFire = colorFire;
            ColorFade = colorFade;
            Scale = scale;
            Opacity = opacity;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            Spin = rotationSpeed;
            Variant = Main.rand.Next(3);
        }

        public override void Update()
        {
            UpdateMediumMist(ref Opacity, ColorFire, ColorFade, Spin);
        }
    }
}

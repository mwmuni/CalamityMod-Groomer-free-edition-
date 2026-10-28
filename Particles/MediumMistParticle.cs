using Microsoft.Xna.Framework;
using Terraria;


namespace CalamityMod.Particles
{
    public class MediumMistParticle : Particle
    {
        public override string Texture => "CalamityMod/Particles/MediumMist";
        public override int FrameVariants => 3;
        public override bool UseAdditiveBlend => true;


        private float Opacity;
        private Color ColorFire;
        private Color ColorFade;
        private float Spin;

        public MediumMistParticle(Vector2 position, Vector2 velocity, Color colorFire, Color colorFade, float scale, float opacity, float rotationSpeed = 0f)
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

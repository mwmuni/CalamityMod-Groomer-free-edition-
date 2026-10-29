using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Particles
{
    public abstract class GravityFadingParticle : Particle
    {
        public Color InitialColor;
        public bool AffectedByGravity;
        public float ExtraRotation;

        protected virtual float ScaleDecay => 0.95f;
        protected virtual Vector2 DrawScale => new Vector2(0.5f, 1.6f);
        protected virtual bool DrawInnerLayer => true;
        protected virtual bool IncludeExtraRotation => false;
        protected virtual bool UseScalarDrawScale => false;
        protected virtual bool ApplyLightingDuringUpdate => false;
        protected virtual bool ApplyLightingDuringDraw => false;

        protected GravityFadingParticle(Vector2 relativePosition, Vector2 velocity, bool affectedByGravity, int lifetime, float scale, Color color, float extraRotation = 0f)
        {
            Position = relativePosition;
            Velocity = velocity;
            AffectedByGravity = affectedByGravity;
            Scale = scale;
            Lifetime = lifetime;
            Color = InitialColor = color;
            ExtraRotation = extraRotation;
        }

        public override bool SetLifetime => true;
        public override bool UseCustomDraw => true;

        public override void Update()
        {
            Scale *= ScaleDecay;
            Color = Color.Lerp(InitialColor, Color.Transparent, (float)Math.Pow(LifetimeCompletion, 3D));
            if (ApplyLightingDuringUpdate && AffectedByLight)
                Color = Lighting.GetColor((Position / 16).ToPoint()).MultiplyRGBA(Color);

            Velocity *= 0.95f;
            if (Velocity.Length() < 12f && AffectedByGravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }

            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
            if (IncludeExtraRotation)
                Rotation += ExtraRotation;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Color drawColor = Color;
            if (ApplyLightingDuringDraw && AffectedByLight)
                drawColor = Color.MultiplyRGB(Lighting.GetColor((Position / 16).ToPoint()));

            Vector2 scale = DrawScale * Scale;
            if (UseScalarDrawScale)
                spriteBatch.Draw(texture, Position - Main.screenPosition, null, drawColor, Rotation, texture.Size() * 0.5f, Scale, SpriteEffects.None, 0f);
            else
                spriteBatch.Draw(texture, Position - Main.screenPosition, null, drawColor, Rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            if (DrawInnerLayer)
                spriteBatch.Draw(texture, Position - Main.screenPosition, null, drawColor, Rotation, texture.Size() * 0.5f, scale * new Vector2(0.45f, 1f), SpriteEffects.None, 0f);
        }
    }
}

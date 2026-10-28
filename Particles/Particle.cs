using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;

namespace CalamityMod.Particles
{
    public abstract class Particle
    {
        /// <summary>
        /// The ID of the particle type as registered by <see cref="GeneralParticleHandler"/> when the mod loads.
        /// <br>This field is set automatically when a particle instance is spawned in the world. This should NOT be set outside of that context.</br>
        /// </summary>
        public int Type;

        /// <summary>
        /// The amount of frames this particle has existed for. You shouldn't have to touch this manually.
        /// </summary>
        public int Time;

        /// <summary>
        /// The maximum amount of frames a particle may stay alive if Particle.SetLifeTime is set to true
        /// </summary>
        public int Lifetime = 0;

        /// <summary>
        /// The offset of the particle in relation to the origin of the set it belongs to. This is only used in the context of a <see cref="BaseParticleSet"/>.
        /// </summary>
        public Vector2 RelativeOffset;

        /// <summary>
        /// The inworld position of a particle. Keep in mind this isn't used in the context of a <see cref="BaseParticleSet"/>, since all the particles work off their relative position to the set's origin
        /// </summary>
        public Vector2 Position;

        public Vector2 Velocity;

        public Vector2 Origin;

        public Color Color;

        public float Rotation;

        public float Scale;

        /// <summary>
        /// The current Y-frame of this particle's spritesheet.
        /// </summary>
        public int Variant = 0;

        /// <summary>
        /// Whether your particle is affected by light levels.
        /// </summary>
        public bool AffectedByLight = false;

        /// <summary>
        /// Whether or not your particle should be drawn with a pixelated effect to match Terraria's pixel size.
        /// <br>Defaults to false.</br>
        /// </summary>
        public bool Pixelate = false;

        /// <summary>
        /// The "layer" or point at which you'd like your particle to draw in Terraria's internal draw order.
        /// <br>Defaults to <see cref="GeneralDrawLayer.AfterDusts"/>.</br>
        /// </summary>
        public GeneralDrawLayer DrawLayer = GeneralDrawLayer.AfterDusts;

        /// <summary>
        /// An 0-1 interpolant representing how close this particle is from its <see cref="Lifetime"/>.
        /// </summary>
        public float LifetimeCompletion => Lifetime != 0 ? Time / (float)Lifetime : 0;

        /// <summary>
        /// The path to this particle's autoloaded texture.
        /// </summary>
        /// <remarks>
        /// Can be accessed via <see cref="GeneralParticleHandler.GetTexture(int)"/>.
        /// </remarks>
        public virtual string Texture => "";

        /// <summary>
        /// <see cref="AssetRequestMode"/> for how <see cref="Texture"/> should be autoloaded.<br/>
        /// Defaults to <see cref="AssetRequestMode.AsyncLoad"/>
        /// </summary>
        public virtual AssetRequestMode TextureRequestMode => AssetRequestMode.AsyncLoad;

        /// <summary>
        /// The maximum amount of frames this particle's spritesheet has vertically.
        /// </summary>
        public virtual int FrameVariants => 1;

        /// <summary>
        /// Set this to true if you NEED the particle to render even if the particle cap is reached.
        /// </summary>
        public virtual bool Important => false;

        /// <summary>
        /// Set this to true if you want your particle to automatically get removed when its time reaches its maximum lifetime
        /// </summary>
        public virtual bool SetLifetime => false;

        /// <summary>
        /// Set this to true to make your particle use additive blending instead of alphablend.
        /// </summary>
        public virtual bool UseAdditiveBlend => false;

        /// <summary>
        /// Set this to true to make your particles work with semi transparent pixels. Is overriden if UseAdditiveBlend is set to true.
        /// </summary>
        public virtual bool UseHalfTransparency => false;

        /// <summary>
        /// Set this to true to disable default particle drawing, thus calling Particle.CustomDraw() instead.
        /// </summary>
        public virtual bool UseCustomDraw => false;

        /// <summary>
        /// An optional shader parameter for this paricle to be drawn with.
        /// Use <see cref="PrepareCustomShader(Effect)"/> to set shader parameters accordingly.
        /// </summary>
        public virtual Effect CustomShader => null;

        /// <summary>
        /// Use this method if you want to handle the particle drawing yourself. Only called if <b><see cref="UseCustomDraw"/></b> is set to true.
        /// </summary>
        public virtual void CustomDraw(SpriteBatch spriteBatch) { }

        /// <summary>
        /// Use this method if you want to handle particle drawing yourself in the context of a <see cref="BaseParticleSet"/>. 
        /// </summary>
        /// <param name="basePosition">The base position of the particle set.</param>
        public virtual void CustomDraw(SpriteBatch spriteBatch, Vector2 basePosition) { }

        /// <summary>
        /// Use this method if you're using a custom shader effect with this particle type to set shader parameters accordingly.
        /// </summary>
        public virtual void PrepareCustomShader(Effect shader) { }

        /// <summary>
        /// Called every frame in <see cref="GeneralParticleHandler.Update"/>.
        /// The particle's velocity gets automatically added to its position, and its time automatically increases.
        /// </summary>
        public virtual void Update() { }

        internal void UpdateMediumMist(ref float opacity, Color colorFire, Color colorFade, float spin)
        {
            Rotation += spin * ((Velocity.X > 0) ? 1f : -1f);
            Velocity *= 0.85f;

            if (opacity > 90)
            {
                Lighting.AddLight(Position, Color.ToVector3() * 0.1f);
                Scale += 0.01f;
                opacity -= 3;
            }
            else
            {
                Scale *= 0.975f;
                opacity -= 2;
            }

            if (opacity < 0)
                Kill();

            Color = Color.Lerp(colorFire, colorFade, MathHelper.Clamp((float)((255 - opacity) - 100) / 80, 0f, 1f)) * (opacity / 255f);
        }

        /// <summary>
        /// Removes the particle from the handler
        /// </summary>
        public void Kill() => GeneralParticleHandler.RemoveParticle(this);
    }
}

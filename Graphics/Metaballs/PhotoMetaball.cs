using System.Collections.Generic;
using System.Linq;
using CalamityMod.Effects;
using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Graphics.Metaballs
{
    public class PhotoMetaball : Metaball
    {
        public class PhotoParticle
        {
            public Vector2 Center;
            public float Size;
            public int Time = 0;
            public float OrginSize;

            public PhotoParticle(Vector2 center, float size)
            {
                Center = center;
                Size = size;
            }

            public void Update()
            {
                Time++;
                if (Time == 1)
                    OrginSize = Size;
                // Always slowly shrink the particles.
                Size = MathHelper.Clamp(Size - 0.8f, 0f, 1000f) * 0.96f;

                // Once sufficiently small, the particles very rapidly shrink.
                if (Size < OrginSize * 0.15f)
                    Size = 2;
            }
        }

        public static List<PhotoParticle> Particles
        {
            get;
            private set;
        } = new();

        public override bool AnythingToDraw => Particles.Any();

        public override IEnumerable<Texture2D> Layers
        {
            get
            {
                yield return ModContent.Request<Texture2D>("CalamityMod/Projectiles/InvisibleProj").Value;
            }
        }

        public override GeneralDrawLayer DrawLayer => GeneralDrawLayer.AfterProjectiles;

        public Color sparkColor;
        public int Time = 0;
        public override Color EdgeColor => sparkColor;

        public override void Update()
        {
            Time++;
            // Update all particle instances.
            // Once sufficiently small, they vanish.
            for (int i = 0; i < Particles.Count; i++)
                Particles[i].Update();
            Particles.RemoveAll(p => p.Size <= 2f);
            if (Time % 10 == 0)
            {
                sparkColor = Main.rand.Next(4) switch
                {
                    0 => Color.Red,
                    1 => Color.MediumTurquoise,
                    2 => Color.Orange,
                    _ => Color.LawnGreen,
                };
            }
        }

        // Copied from Rancor Lava metaballs, since these need to be additive metaballs.
        public override void PrepareShaderForTarget(int layerIndex)
        {
            AdditiveMetaballShaderHelper.Prepare();
        }

        public static void SpawnParticle(Vector2 position, float size) => Particles.Add(new(position, size));

        public override void DrawInstances()
        {
            float opacity = 1f;
            Texture2D tex = ModContent.Request<Texture2D>("CalamityMod/Graphics/Metaballs/MetaballMessy").Value;

            foreach (PhotoParticle particle in Particles)
            {
                Vector2 drawPosition = particle.Center - Main.screenPosition;
                var origin = tex.Size() * 0.5f;
                Vector2 scale = Vector2.One * particle.Size / tex.Size();

                float Interpolant = Utils.GetLerpValue(25f, 60f, particle.Size * 0.6f, true);
                Color drawColor = Color.Lerp(EdgeColor, Color.White * 0.9f, Interpolant).MultiplyRGBA(new Color(1f, 1f, 1f, opacity));

                Main.spriteBatch.Draw(tex, drawPosition, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}

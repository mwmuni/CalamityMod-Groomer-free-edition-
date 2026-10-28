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
    public class PhotoMetaball3 : Metaball
    {
        public class PhotoParticle3
        {
            public Vector2 Center;
            public float Size;
            public int Time = 0;

            public PhotoParticle3(Vector2 center, float size)
            {
                Center = center;
                Size = size;
            }

            public void Update()
            {
                Time++;
                if (Time >= 6)
                    Size = 2;
            }
        }

        public static List<PhotoParticle3> Particles
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

            List<Color> eColors = new List<Color>()
            {
                Color.OrangeRed,
                Color.MediumTurquoise,
                Color.Orange,
                Color.LawnGreen
            };
            float rate = (Main.GlobalTimeWrappedHourly * 8);
            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            sparkColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);
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

            foreach (PhotoParticle3 particle in Particles)
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

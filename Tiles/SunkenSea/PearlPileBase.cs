using System;
using CalamityMod.Sounds;
using CalamityMod.Systems;
using CalamityMod.Tiles.Abyss;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public abstract class PearlPileBase : ModTile
    {
        public Asset<Texture2D> GlintTexture;
        public Vector2 GlintDir;

        protected abstract int PearlDust { get; }
        protected abstract int FallingProjectile { get; }
        protected abstract Color PearlMapColor { get; }
        protected abstract Vector3 PearlLightColor { get; }

        protected virtual bool BlendAll => false;

        public override void SetStaticDefaults()
        {
            GlintTexture = ModContent.Request<Texture2D>(Texture + "_Glint");

            GlintDir = new Vector2(1f, -1f);
            GlintDir.Normalize();

            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            if (BlendAll)
                Main.tileBlendAll[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);

            TileID.Sets.HasSlopeFrames[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            HitSound = CommonCalamitySounds.VoidstoneMine;
            DustType = PearlDust;
            AddMapEntry(PearlMapColor);
            Main.tileShine[Type] = 3500;
            Main.tileShine2[Type] = true;

            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.Falling[Type] = true;
            TileID.Sets.FallingBlockProjectile[Type] = new TileID.Sets.FallingBlockProjectileInfo(FallingProjectile, 15);

            RegisterBlendMerges();
        }

        protected virtual void RegisterBlendMerges()
        {
            this.RegisterBlendMergeWith(ModContent.TileType<Shellstone>());
            this.RegisterBlendMergeWith(ModContent.TileType<AbyssGravel>());
            this.RegisterBlendMergeWith(ModContent.TileType<EutrophicSand>());
            this.RegisterBlendMergeWith(ModContent.TileType<Navystone>());
            this.RegisterBlendMergeWith(ModContent.TileType<Runestone>());
            this.RegisterBlendMergeWith(TileID.Sandstone);
            this.RegisterBlendMergeWith(TileID.Sand);
            this.RegisterBlendMergeWith(TileID.HardenedSand);
            this.RegisterBlendMergeWith(TileID.Stone);
            this.RegisterBlendMergeWith(TileID.Dirt);
            this.RegisterBlendMergeWith(TileID.Ash);
            this.RegisterBlendMergeWith(TileID.Mud);
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return TileFramingSystem.BetterGemsparkFraming(i, j, resetFrame);
        }

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            Main.tile[i, j].Get<TileSpecialDrawData>().Flag0 = !Main.tile[i - 1, j].HasTile || !Main.tile[i + 1, j].HasTile || !Main.tile[i, j - 1].HasTile || !Main.tile[i, j + 1].HasTile;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            //IF this glint effect below runs poorly on lower end PC's we should keep it as a setting for those with good PC's
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 offScreen = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            Vector2 position = new Vector2(i * 16, j * 16) - Main.screenPosition + offScreen;

            int frameX = tile.TileFrameX + (i % 1);
            int frameY = tile.TileFrameY + (j % 1);
            Rectangle sourceRect = new Rectangle(frameX, frameY, 16, 16);

            Vector2 screenPos = position;
            float projection = Vector2.Dot(screenPos, GlintDir);
            float screenDiagonalLength = Vector2.Dot(new Vector2(Main.screenWidth, Main.screenHeight), GlintDir);

            float stripeWidth = 100f;
            Color lightColor = Lighting.GetColor(i, j) * 6;

            DrawGlint(screenDiagonalLength * 0.05f);
            DrawGlint(screenDiagonalLength * 0.5f);
            DrawGlint(screenDiagonalLength * 1.05f);

            void DrawGlint(float beamCenter)
            {
                float dist = Math.Abs(projection - beamCenter);
                float strength = MathHelper.Clamp(1f - dist / stripeWidth, 0f, 1f) * 0.4f;

                if (strength > 0f)
                    spriteBatch.Draw(GlintTexture.Value, position, sourceRect, lightColor * strength, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            if (!Main.tile[i, j].Get<TileSpecialDrawData>().Flag0)
                return;

            Color litColor = Lighting.GetColor(i, j);
            float brightness = (litColor.R + litColor.G + litColor.B) / (3f * 255f);
            float darknessFactor = 1f - brightness;

            Vector2 screenPos = new Vector2(i * 16, j * 16) - Main.screenPosition;
            float projection = Vector2.Dot(screenPos, GlintDir);
            float screenDiagonalLength = Vector2.Dot(new Vector2(Main.screenWidth, Main.screenHeight), GlintDir);

            float stripeWidth = 100f;
            float maxStrength = 0f;

            UpdateMaxStrength(screenDiagonalLength * 0.05f);
            UpdateMaxStrength(screenDiagonalLength * 0.5f);
            UpdateMaxStrength(screenDiagonalLength * 1.05f);

            void UpdateMaxStrength(float beamCenter)
            {
                float dist = Math.Abs(projection - beamCenter);
                float strength = MathHelper.Clamp(1f - dist / stripeWidth, 0f, 1f);
                if (strength > maxStrength)
                    maxStrength = strength;
            }

            if (maxStrength > 0f)
            {
                float intensity = 0.6f * maxStrength * (0.5f + darknessFactor * 0.5f);
                r = PearlLightColor.X * intensity;
                g = PearlLightColor.Y * intensity;
                b = PearlLightColor.Z * intensity;
            }
        }
    }
}

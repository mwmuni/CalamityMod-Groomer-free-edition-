using System;
using CalamityMod.Effects;
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Systems;
using CalamityMod.Utilities.Daybreak;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace CalamityMod.Tiles.SunkenSea
{
    public class SeaPrism : ModTile
    {
        internal const short subsheetWidth = 468;
        internal const short subsheetHeight = 90;

        internal static Asset<Texture2D> Blue;
        internal static Asset<Texture2D> Purple;
        internal static Asset<Texture2D> Green;
        internal static Asset<Texture2D> Glint;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = false;
            TileID.Sets.HasSlopeFrames[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<MediumSeaPrismCrystal>()] = true;

            Main.tileMerge[Type][ModContent.TileType<SeaPrismCrystals>()] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);
            Main.tileLighted[Type] = true;
            Main.tileShine2[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            DustType = DustID.Water;
            AddMapEntry(new Color(97, 212, 223));
            HitSound = SoundID.Tink;
            Main.tileSpelunker[Type] = true;
            MinPick = 55;

            this.RegisterBlendMergeWith(ModContent.TileType<Navystone>());
            this.RegisterBlendMergeWith(ModContent.TileType<EutrophicSand>());

            Blue = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/SeaPrism_Blue");
            Purple = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/SeaPrism_Purple");
            Green = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/SeaPrism_Green");
            Glint = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/SeaPrism_GlintMask");
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 8 * subsheetWidth;
            frameYOffset = j % 8 * subsheetHeight;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
            => ApplyPrismLight(i, j, ref r, ref g, ref b);

        internal static void ApplyPrismLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float fade1 = GetFade1(i, j);
            float fade2 = GetFade2(i, j);

            Color baseColor = new Color(162, 216, 218); //Blue
            Color glow1 = new Color(171, 113, 215);    //Purple
            Color glow2 = new Color(56, 174, 117);     //Green

            Vector3 blended = baseColor.ToVector3();

            blended = Vector3.Lerp(blended, glow1.ToVector3(), fade1 * 0.5f);
            blended = Vector3.Lerp(blended, glow2.ToVector3(), fade2 * 0.5f);

            float brightness = 0.6f;
            blended *= brightness;

            r = blended.X;
            g = blended.Y;
            b = blended.Z;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            return TileFramingSystem.BetterGemsparkFraming(i, j, resetFrame);
        }

        internal static float GetFade1(int i, int j) => (MathF.Sin(Main.GlobalTimeWrappedHourly * 0.2f) + 1f) / 2f;

        internal static float GetFade2(int i, int j) => (MathF.Sin(Main.GlobalTimeWrappedHourly * 0.1f + i * 0.08f - j * 0.05f) + 1f) / 2f;

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => false;
    }

    public class SeaPrismWall : MultiVariantModWall
    {
        public override string Texture => "CalamityMod/Walls/SeaPrismWall";

        internal static FramedMaskTexture GlowMaskBlue;
        internal static FramedMaskTexture GlowMaskPurple;
        internal static FramedMaskTexture GlowMaskGreen;
        public override void SetStaticDefaults()
        {
            GlowMaskBlue = new("CalamityMod/Walls/SeaPrismWall_Blue", 36, 36);
            GlowMaskPurple = new("CalamityMod/Walls/SeaPrismWall_Purple", 36, 36);
            GlowMaskGreen = new("CalamityMod/Walls/SeaPrismWall_Green", 36, 36);
            Main.wallHouse[Type] = true;
            DustType = DustID.RainCloud;

            AddMapEntry(new Color(27, 123, 131));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void PopulateWallVariant(int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = (i % 8) * 468;
            frameYOffset = (j % 8) * 180;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => false;
    }

    public class UnsafeSeaPrismWall : MultiVariantModWall
    {
        public override string Texture => "CalamityMod/Walls/SeaPrismWall";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustID.RainCloud;

            AddMapEntry(new Color(27, 123, 131));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

        public override void PopulateWallVariant(int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = (i % 8) * 468;
            frameYOffset = (j % 8) * 180;
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => false;

        public override bool Drop(int i, int j, ref int type) => false;
    }

    public class SeaPrismCrystals : ModTile
    {
        internal static Asset<Texture2D> BlueCrystals;
        internal static Asset<Texture2D> PurpleCrystals;
        internal static Asset<Texture2D> GreenCrystals;
        internal static Asset<Texture2D> Glint;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(53, 136, 207), CalamityUtils.GetItemName<PrismShard>());
            HitSound = SoundID.Item27;
            DustType = DustID.IceRod;
            Main.tileSpelunker[Type] = true;
            MinPick = 55;

            BlueCrystals = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/SeaPrismCrystals");
            PurpleCrystals = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/SeaPrismCrystals_Purple");
            GreenCrystals = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/SeaPrismCrystals_Green");
            Glint = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/SeaPrismCrystals_Glint");
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
            => SeaPrism.ApplyPrismLight(i, j, ref r, ref g, ref b);

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => false;

        public override bool CanPlace(int i, int j)
        {
            Tile belowTile = Main.tile[i, j + 1];
            Tile aboveTile = Main.tile[i, j - 1];
            Tile rightTile = Main.tile[i + 1, j];
            Tile leftTile = Main.tile[i - 1, j];

            if ((belowTile.Slope == SlopeType.Solid && !belowTile.IsHalfBlock && belowTile.HasTile && belowTile.IsTileSolid()) ||
                (aboveTile.Slope == SlopeType.Solid && !aboveTile.IsHalfBlock && aboveTile.HasTile && aboveTile.IsTileSolid()) ||
                (rightTile.Slope == SlopeType.Solid && !rightTile.IsHalfBlock && rightTile.HasTile && rightTile.IsTileSolid()) ||
                (leftTile.Slope == SlopeType.Solid && !leftTile.IsHalfBlock && leftTile.HasTile && leftTile.IsTileSolid()))
                return true;

            return false;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            Tile belowTile = Main.tile[i, j + 1];
            Tile aboveTile = Main.tile[i, j - 1];
            Tile rightTile = Main.tile[i + 1, j];
            Tile leftTile = Main.tile[i - 1, j];

            if (belowTile.Slope == SlopeType.Solid && !belowTile.IsHalfBlock && belowTile.HasTile && belowTile.IsTileSolid())
                Main.tile[i, j].TileFrameY = 0;
            else if (aboveTile.Slope == SlopeType.Solid && !aboveTile.IsHalfBlock && aboveTile.HasTile && aboveTile.IsTileSolid())
                Main.tile[i, j].TileFrameY = 18;
            else if (rightTile.Slope == SlopeType.Solid && !rightTile.IsHalfBlock && rightTile.HasTile && rightTile.IsTileSolid())
                Main.tile[i, j].TileFrameY = 36;
            else if (leftTile.Slope == SlopeType.Solid && !leftTile.IsHalfBlock && leftTile.HasTile && leftTile.IsTileSolid())
                Main.tile[i, j].TileFrameY = 54;

            Main.tile[i, j].TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
        }
    }

    public class MediumSeaPrismCrystal : ModTile
    {
        internal static Asset<Texture2D> BlueCrystals;
        internal static Asset<Texture2D> PurpleCrystals;
        internal static Asset<Texture2D> GreenCrystals;
        internal static Asset<Texture2D> Glint;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileObsidianKill[Type] = true;
            Main.tileSpelunker[Type] = true;
            Main.tileShine[Type] = 5600;
            Main.tileShine2[Type] = true;

            HitSound = SoundID.Item27;
            DustType = DustID.IceRod;
            MinPick = 55;

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AddMapEntry(new Color(53, 136, 207), CalamityUtils.GetItemName<PrismShard>());

            // Attach to ground
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.StyleMultiplier = 32; // total 32 frames, all should be same "itemStyle"
            TileObjectData.newTile.StyleWrapLimit = 8; // only 1 placement alternative per row
            TileObjectData.newTile.RandomStyleRange = 8; // 8 different style will be selected upon placing
            TileObjectData.newTile.Origin = new Point16(0, 1);

            // Attach to side (right)
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.Origin = new Point16(1, 0);
            TileObjectData.addAlternate(8);

            // Attach to ceiling
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.Origin = new Point16(0, 0);
            TileObjectData.addAlternate(16);

            // Attach to side (left)
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
            TileObjectData.newAlternate.Origin = new Point16(0, 0);
            TileObjectData.addAlternate(24);
            TileObjectData.addTile(Type);

            BlueCrystals = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/MediumSeaPrismCrystal_Blue");
            PurpleCrystals = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/MediumSeaPrismCrystal_Purple");
            GreenCrystals = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/MediumSeaPrismCrystal_Green");
            Glint = ModContent.Request<Texture2D>("CalamityMod/Tiles/SunkenSea/MediumSeaPrismCrystal_Glint");
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
            => SeaPrism.ApplyPrismLight(i, j, ref r, ref g, ref b);

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) => false;

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 48, ModContent.ItemType<PrismShard>(), 4);
        }
    }

    public class SeaPrismShaderDrawing : ModSystem
    {
        private int SeaPrismTileType = -1;
        private int SeaPrismCrystalTileType = -1;
        private int MediumSeaPrismCrystalTileType = -1;

        private int SeaPrismWallType = -1;
        private int UnsafeSeaPrismWallType = -1;

        public override void OnModLoad()
        {
            if (!Main.dedServ)
            {
                Main.QueueMainThreadAction(() =>
                {
                    On_Main.DrawTiles += DrawSeaPrismsAndCrystals;
                    On_Main.DrawWalls += DrawSeaPrismWalls;
                });
            }

            SeaPrismTileType = ModContent.TileType<SeaPrism>();
            SeaPrismCrystalTileType = ModContent.TileType<SeaPrismCrystals>();
            MediumSeaPrismCrystalTileType = ModContent.TileType<MediumSeaPrismCrystal>();

            SeaPrismWallType = ModContent.WallType<SeaPrismWall>();
            UnsafeSeaPrismWallType = ModContent.WallType<UnsafeSeaPrismWall>();
        }

        private static void GetScreenDrawArea(Vector2 screenPosition, Vector2 offSet, out int firstTileX, out int lastTileX, out int firstTileY, out int lastTileY)
        {
            firstTileX = (int)((screenPosition.X - offSet.X) / 16f - 1f);
            lastTileX = (int)((screenPosition.X + (float)Main.screenWidth + offSet.X) / 16f) + 2;
            firstTileY = (int)((screenPosition.Y - offSet.Y) / 16f - 1f);
            lastTileY = (int)((screenPosition.Y + (float)Main.screenHeight + offSet.Y) / 16f) + 5;
            if (firstTileX < 4)
            {
                firstTileX = 4;
            }
            if (lastTileX > Main.maxTilesX - 4)
            {
                lastTileX = Main.maxTilesX - 4;
            }
            if (firstTileY < 4)
            {
                firstTileY = 4;
            }
            if (lastTileY > Main.maxTilesY - 4)
            {
                lastTileY = Main.maxTilesY - 4;
            }
        }

        private void DrawSeaPrismsAndCrystals(On_Main.orig_DrawTiles orig, Main self, bool solidLayer, bool forRenderTargets, bool intoRenderTargets, int waterStyleOverride)
        {
            Vector2 offscreenPosition = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            var oldTex1 = Main.instance.GraphicsDevice.Textures[1];
            var oldSampler1 = Main.instance.GraphicsDevice.SamplerStates[1];
            var oldTex2 = Main.instance.GraphicsDevice.Textures[2];
            var oldSampler2 = Main.instance.GraphicsDevice.SamplerStates[2];
            var oldTex3 = Main.instance.GraphicsDevice.Textures[3];
            var oldSampler3 = Main.instance.GraphicsDevice.SamplerStates[3];

            Effect shader = CalamityShaders.SeaPrismColorBlendingShader.Value;
            shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
            shader.Parameters["screenOffset"].SetValue(Main.screenPosition);
            shader.Parameters["offscreenOffset"].SetValue(offscreenPosition);
            shader.Parameters["diagonalScreenLength"].SetValue((Main.screenWidth / 2f) - (Main.screenHeight / 2f));
            shader.Parameters["doGlint"].SetValue(true);

            Vector2 unscaledPosition = Main.Camera.UnscaledPosition;
            GetScreenDrawArea(unscaledPosition, offscreenPosition + (Main.Camera.UnscaledPosition - Main.Camera.ScaledPosition), out var firstTileX, out var lastTileX, out var firstTileY, out var lastTileY);

            if ((solidLayer && !Main.drawToScreen) || (!solidLayer && Main.drawToScreen))
            {
                Main.tileBatch.End();
                Main.spriteBatch.End(out var snapshot);

                Main.instance.GraphicsDevice.Textures[1] = SeaPrism.Green.Value;
                Main.instance.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
                Main.instance.GraphicsDevice.Textures[2] = SeaPrism.Purple.Value;
                Main.instance.GraphicsDevice.SamplerStates[2] = SamplerState.LinearClamp;
                Main.instance.GraphicsDevice.Textures[3] = SeaPrism.Glint.Value;
                Main.instance.GraphicsDevice.SamplerStates[3] = SamplerState.LinearClamp;

                Main.tileBatch.Begin(snapshot.RasterizerState, snapshot.TransformMatrix);
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, snapshot.RasterizerState, shader, snapshot.TransformMatrix);

                for (int y = firstTileY; y < lastTileY + 4; y++)
                {
                    for (int x = firstTileX - 2; x < lastTileX + 2; x++)
                    {
                        if (!WorldGen.InWorld(x, y))
                            continue;

                        Tile tile = Main.tile[x, y];
                        int type = tile.TileType;
                        if (type != SeaPrismTileType)
                            continue;

                        Vector2 position = new Vector2(x * 16, y * 16) - Main.screenPosition + offscreenPosition;

                        int frameX = tile.TileFrameX + (x % 8 * SeaPrism.subsheetWidth);
                        int frameY = tile.TileFrameY + (y % 8 * SeaPrism.subsheetHeight);

                        Rectangle sourceRect = new Rectangle(frameX, frameY, 16, 16);
                        Color light = Lighting.GetColor(x, y) * 1.5f;
                        if (tile.IsActuated) light = light.MultiplyRGB(Color.White * 0.4f);

                        Main.spriteBatch.Draw(SeaPrism.Blue.Value, position, sourceRect, light);
                    }
                }

                Main.tileBatch.End();
                Main.spriteBatch.End();

                Main.instance.GraphicsDevice.Textures[1] = oldTex1;
                Main.instance.GraphicsDevice.SamplerStates[1] = oldSampler1;
                Main.instance.GraphicsDevice.Textures[2] = oldTex2;
                Main.instance.GraphicsDevice.SamplerStates[2] = oldSampler2;
                Main.instance.GraphicsDevice.Textures[3] = oldTex3;
                Main.instance.GraphicsDevice.SamplerStates[3] = oldSampler3;

                Main.tileBatch.Begin(snapshot.RasterizerState, snapshot.TransformMatrix);
                Main.spriteBatch.Begin(snapshot);
            }

            if (!solidLayer)
            {
                Main.tileBatch.End();
                Main.spriteBatch.End(out var snapshot);

                Main.instance.GraphicsDevice.Textures[1] = SeaPrismCrystals.GreenCrystals.Value;
                Main.instance.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
                Main.instance.GraphicsDevice.Textures[2] = SeaPrismCrystals.PurpleCrystals.Value;
                Main.instance.GraphicsDevice.SamplerStates[2] = SamplerState.LinearClamp;
                Main.instance.GraphicsDevice.Textures[3] = SeaPrismCrystals.Glint.Value;
                Main.instance.GraphicsDevice.SamplerStates[3] = SamplerState.LinearClamp;

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, snapshot.RasterizerState, shader, snapshot.TransformMatrix);

                for (int y = firstTileY; y < lastTileY + 4; y++)
                {
                    for (int x = firstTileX - 2; x < lastTileX + 2; x++)
                    {
                        if (!WorldGen.InWorld(x, y))
                            continue;

                        Tile tile = Main.tile[x, y];
                        int type = tile.TileType;
                        if (type != SeaPrismCrystalTileType)
                            continue;

                        Vector2 position = new Vector2(x * 16, y * 16) - Main.screenPosition + offscreenPosition;

                        Rectangle sourceRect = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
                        Color light = Lighting.GetColor(x, y) * 1.5f;

                        Main.spriteBatch.Draw(SeaPrismCrystals.BlueCrystals.Value, position, sourceRect, light);
                    }
                }

                Main.spriteBatch.End();

                Main.instance.GraphicsDevice.Textures[1] = MediumSeaPrismCrystal.GreenCrystals.Value;
                Main.instance.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
                Main.instance.GraphicsDevice.Textures[2] = MediumSeaPrismCrystal.PurpleCrystals.Value;
                Main.instance.GraphicsDevice.SamplerStates[2] = SamplerState.LinearClamp;
                Main.instance.GraphicsDevice.Textures[3] = MediumSeaPrismCrystal.Glint.Value;
                Main.instance.GraphicsDevice.SamplerStates[3] = SamplerState.LinearClamp;

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, snapshot.RasterizerState, shader, snapshot.TransformMatrix);

                for (int y = firstTileY; y < lastTileY + 4; y++)
                {
                    for (int x = firstTileX - 2; x < lastTileX + 2; x++)
                    {
                        if (!WorldGen.InWorld(x, y))
                            continue;

                        Tile tile = Main.tile[x, y];
                        int type = tile.TileType;
                        if (type != MediumSeaPrismCrystalTileType)
                            continue;

                        Vector2 position = new Vector2(x * 16, y * 16) - Main.screenPosition + offscreenPosition;

                        Rectangle sourceRect = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
                        Color light = Lighting.GetColor(x, y) * 1.5f;

                        Main.spriteBatch.Draw(MediumSeaPrismCrystal.BlueCrystals.Value, position, sourceRect, light);
                    }
                }

                Main.spriteBatch.End();

                Main.instance.GraphicsDevice.Textures[1] = oldTex1;
                Main.instance.GraphicsDevice.SamplerStates[1] = oldSampler1;
                Main.instance.GraphicsDevice.Textures[2] = oldTex2;
                Main.instance.GraphicsDevice.SamplerStates[2] = oldSampler2;
                Main.instance.GraphicsDevice.Textures[3] = oldTex3;
                Main.instance.GraphicsDevice.SamplerStates[3] = oldSampler3;

                Main.tileBatch.Begin(snapshot.RasterizerState, snapshot.TransformMatrix);
                Main.spriteBatch.Begin(snapshot);
            }

            orig(self, solidLayer, forRenderTargets, intoRenderTargets, waterStyleOverride);
        }

        private void DrawSeaPrismWalls(On_Main.orig_DrawWalls orig, Main self)
        {
            Vector2 offscreenPosition = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            Main.tileBatch.End();
            Main.spriteBatch.End(out var snapshot);

            var oldTex1 = Main.instance.GraphicsDevice.Textures[1];
            var oldSampler1 = Main.instance.GraphicsDevice.SamplerStates[1];
            var oldTex2 = Main.instance.GraphicsDevice.Textures[2];
            var oldSampler2 = Main.instance.GraphicsDevice.SamplerStates[2];
            var oldTex3 = Main.instance.GraphicsDevice.Textures[3];

            Main.instance.GraphicsDevice.Textures[1] = SeaPrismWall.GlowMaskGreen.Texture;
            Main.instance.GraphicsDevice.SamplerStates[1] = SamplerState.LinearClamp;
            Main.instance.GraphicsDevice.Textures[2] = SeaPrismWall.GlowMaskPurple.Texture;
            Main.instance.GraphicsDevice.SamplerStates[2] = SamplerState.LinearClamp;
            Main.instance.GraphicsDevice.Textures[3] = null;

            Effect shader = CalamityShaders.SeaPrismColorBlendingShader.Value;
            shader.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly);
            shader.Parameters["screenOffset"].SetValue(Main.screenPosition);
            shader.Parameters["offscreenOffset"].SetValue(offscreenPosition);
            shader.Parameters["diagonalScreenLength"].SetValue((Main.screenWidth / 2f) - (Main.screenHeight / 2f));
            shader.Parameters["doGlint"].SetValue(false);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, snapshot.RasterizerState, null, snapshot.TransformMatrix);

            Vector2 unscaledPosition = Main.Camera.UnscaledPosition;

            GetScreenDrawArea(unscaledPosition, offscreenPosition + (Main.Camera.UnscaledPosition - Main.Camera.ScaledPosition), out var firstTileX, out var lastTileX, out var firstTileY, out var lastTileY);

            for (int y = firstTileY; y < lastTileY + 4; y++)
            {
                for (int x = firstTileX - 2; x < lastTileX + 2; x++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (tile == null)
                        continue;

                    int type = tile.WallType;
                    if (type != SeaPrismWallType && type != UnsafeSeaPrismWallType)
                        continue;

                    Vector2 position = new Vector2(x * 16, y * 16) - Main.screenPosition + offscreenPosition;

                    int xLength = 32;

                    int frameXOffset = (x % 8) * 468;
                    int frameYOffset = (y % 8) * 180;

                    int xPos = tile.WallFrameX + frameXOffset;
                    int yPos = tile.WallFrameY + frameYOffset;

                    Rectangle frame = new Rectangle(xPos, yPos, xLength, 32);

                    if (SeaPrismWall.GlowMaskBlue.HasContentInFramePos(xPos, yPos))
                        Main.spriteBatch.Draw(SeaPrismWall.GlowMaskBlue.Texture, position + new Vector2(-8, -8), frame, Color.White * 1.1f);
                }
            }

            Main.spriteBatch.End();

            Main.instance.GraphicsDevice.Textures[1] = oldTex1;
            Main.instance.GraphicsDevice.SamplerStates[1] = oldSampler1;
            Main.instance.GraphicsDevice.Textures[2] = oldTex2;
            Main.instance.GraphicsDevice.SamplerStates[2] = oldSampler2;
            Main.instance.GraphicsDevice.Textures[3] = oldTex3;

            Main.tileBatch.Begin();
            Main.spriteBatch.Begin(snapshot);

            orig(self);
        }
    }
}

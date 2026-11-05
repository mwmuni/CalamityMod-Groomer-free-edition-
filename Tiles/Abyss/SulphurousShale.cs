using CalamityMod.Systems;
using CalamityMod.Tiles.Abyss.AbyssAmbient;
using CalamityMod.Waters;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Abyss
{
    public class SulphurousShale : ModTile
    {
        int animationFrameWidth = 234;

        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/AbyssGravelMine", 3);

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMergeDirt[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithAbyss(Type);

            AddMapEntry(new Color(57, 44, 93));
            MineResist = 3f;
            MinPick = 65;
            HitSound = MineSound;
            DustType = DustID.Water;
            this.RegisterBlendMergeWith(TileID.Dirt);
            this.RegisterBlendMergeWith(TileID.Stone);
            this.RegisterBlendMergeWith(ModContent.TileType<AbyssGravel>());
        }
        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            World.Abyss.FillTileWithWater(i, j);
        }

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (Main.dedServ)
                return;

            if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<BiomeManagers.AbyssLayer1Biome>()))
            {
                Main.SceneMetrics.ActiveFountainColor = SulphuricDepthsWater.Instance.Slot;
            }
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void RandomUpdate(int i, int j)
        {
            int vineLength = WorldGen.genRand.Next((int)Main.rockLayer, (int)(Main.rockLayer + (double)Main.maxTilesY * 0.143));

            if (Main.tile[i, j + 1] != null && j >= SulphurousSea.VineGrowTopLimit)
            {
                // The nearby vine scan is by far the most expensive check, so it runs last.
                if (!Main.tile[i, j + 1].HasTile && Main.tile[i, j + 1].TileType != (ushort)ModContent.TileType<SulphurousVines>() && !SulphurousVines.TooManyVinesNearby(i, j))
                {
                    if (Main.tile[i, j + 1].LiquidAmount == 255 &&
                        Main.tile[i, j + 1].LiquidType != LiquidID.Lava)
                    {
                        bool canGrowVine = false;
                        for (int k = vineLength; k > vineLength - 10; k--)
                        {
                            if (Main.tile[i, k].BottomSlope)
                            {
                                canGrowVine = false;
                                break;
                            }
                            if (Main.tile[i, k].HasTile && !Main.tile[i, k].BottomSlope)
                            {
                                canGrowVine = true;
                                break;
                            }
                        }
                        if (canGrowVine)
                        {
                            int vineX = i;
                            int vineY = j + 1;
                            Main.tile[vineX, vineY].TileType = (ushort)ModContent.TileType<SulphurousVines>();
                            Main.tile[vineX, vineY].Get<TileWallWireStateData>().HasTile = true;
                            WorldGen.SquareTileFrame(vineX, vineY, true);
                            if (Main.dedServ)
                                NetMessage.SendTileSquare(-1, vineX, vineY, 3, TileChangeType.None);
                        }
                        Main.tile[i, j].Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                        Main.tile[i, j].Get<TileWallWireStateData>().IsHalfBlock = false;
                    }
                }
            }

            Tile tile = Main.tile[i, j];
            Tile up = Main.tile[i, j - 1];
            Tile up2 = Main.tile[i, j - 2];

            // Place sulphur tentacle corals
            if (WorldGen.genRand.NextBool(10) && !up.HasTile && !up2.HasTile && up.LiquidAmount > 0 && up2.LiquidAmount > 0 && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
            {
                up.TileType = (ushort)ModContent.TileType<SulphurTentacleCorals>();
                up.HasTile = true;
                up.TileFrameY = 0;

                // 18 different frames, choose a random one
                up.TileFrameX = (short)(WorldGen.genRand.Next(22) * 18);
                WorldGen.SquareTileFrame(i, j - 1, true);

                if (Main.dedServ)
                    NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
            }
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = animationFrameWidth * TileFramingSystem.GetVariation4x4_012_Low0(i, j);
        }
    }
}

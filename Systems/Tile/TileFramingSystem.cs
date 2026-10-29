using System.Collections.Generic;
using System.Linq;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Abyss.AbyssAmbient;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.Tiles.SunkenSea.Ambient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class TileFramingSystem : ModSystem
    {
        // This enum is intentionally not visible to the remainder of the codebase. It is only used here.
        #region Similarity Enum
        private enum Similarity
        {
            Same,
            MergeLink,
            None
        }

        private static Similarity GetSimilarity(Tile check, int myType, int mergeType)
        {
            if (!check.HasTile)
                return Similarity.None;

            if (check.TileType == myType || Main.tileMerge[myType][check.TileType])
                return Similarity.Same;
            else if (check.TileType == mergeType)
                return Similarity.MergeLink;

            return Similarity.None;
        }
        #endregion

        // This tiny GlobalTile exists for two purposes:
        // 1 - Ensure merge data is loaded and cached
        // 2 - Properly and automatically render merges in all cases for all tiles
        #region Sealed Global Tile
        private sealed class MergeableTileGlobalTile : GlobalTile
        {
            public override bool TileFrame(int i, int j, int type, ref bool resetFrame, ref bool noBreak)
            {
                // Custom plant framing
                for (int k = 0; k < PlantTypes.Length; k++)
                    if (type == PlantTypes[k])
                    {
                        PlantFrame(i, j);
                        return false;
                    }

                // Custom vine framing
                if (type == TileID.Vines || type == TileID.CrimsonVines || type == TileID.HallowedVines || type == ModContent.TileType<AstralVines>())
                    VineFrame(i, j);

                return base.TileFrame(i, j, type, ref resetFrame, ref noBreak);
            }
        }
        #endregion

        // CONSIDER -- This is a triangle array, but does it need to be? Main.tileMerge is a triangle array as well
        public static bool[][] tileMergeTypes;

        #region Plant Stuff
        private static ushort[] PlantTypes =
        [
            TileID.Plants,
            TileID.CorruptPlants,
            TileID.JunglePlants,
            TileID.MushroomPlants,
            TileID.Plants2,
            TileID.JunglePlants2,
            TileID.HallowedPlants,
            TileID.HallowedPlants2,
            TileID.CrimsonPlants,
            (ushort)ModContent.TileType<AstralShortPlants>(),
            (ushort)ModContent.TileType<AstralTallPlants>(),
            (ushort)ModContent.TileType<LavaPistil>(),
            (ushort)ModContent.TileType<CinderBlossomTallPlants>(),
            (ushort)ModContent.TileType<SulphurTentacleCorals>(),
            (ushort)ModContent.TileType<AbyssKelp>(),
            (ushort)ModContent.TileType<TenebrisRemnant>(),
            (ushort)ModContent.TileType<PhoviamareHalm>(),
            (ushort)ModContent.TileType<LongScarletSeagrass>(),
            (ushort)ModContent.TileType<SunkenKelp>(),
        ];

        private static int[][] PlantValidGrounds;
        private static Dictionary<ushort, ushort> VineToGrass;
        #endregion

        #region ModSystem Hooks (Load / Unload)

        // All tiles from all mods are guaranteed to be loaded by the execution time of this hook.
        public override void PostAddRecipes()
        {
            PlantValidGrounds = new int[TileLoader.TileCount][];
            PlantValidGrounds[TileID.Plants] = new int[3] { TileID.Grass, TileID.PlanterBox, TileID.ClayPot };
            PlantValidGrounds[TileID.CorruptPlants] = new int[1] { TileID.CorruptGrass };
            PlantValidGrounds[TileID.JunglePlants] = new int[1] { TileID.JungleGrass };
            PlantValidGrounds[TileID.MushroomPlants] = new int[1] { TileID.MushroomGrass };
            PlantValidGrounds[TileID.Plants2] = new int[3] { TileID.Grass, TileID.PlanterBox, TileID.ClayPot };
            PlantValidGrounds[TileID.JunglePlants2] = new int[1] { TileID.JungleGrass };
            PlantValidGrounds[TileID.HallowedPlants] = new int[1] { TileID.HallowedGrass };
            PlantValidGrounds[TileID.HallowedPlants2] = new int[1] { TileID.HallowedGrass };
            PlantValidGrounds[TileID.CrimsonPlants] = new int[1] { TileID.CrimsonGrass };
            PlantValidGrounds[ModContent.TileType<AstralShortPlants>()] = new int[1] { ModContent.TileType<AstralGrass>() };
            PlantValidGrounds[ModContent.TileType<AstralTallPlants>()] = new int[1] { ModContent.TileType<AstralGrass>() };
            PlantValidGrounds[ModContent.TileType<CinderBlossomTallPlants>()] = new int[1] { ModContent.TileType<ScorchedRemainsGrass>() };
            PlantValidGrounds[ModContent.TileType<SulphurTentacleCorals>()] = new int[1] { ModContent.TileType<SulphurousShale>() };
            PlantValidGrounds[ModContent.TileType<AbyssKelp>()] = new int[1] { ModContent.TileType<AbyssGravel>() };
            PlantValidGrounds[ModContent.TileType<TenebrisRemnant>()] = new int[1] { ModContent.TileType<Voidstone>() };
            PlantValidGrounds[ModContent.TileType<PhoviamareHalm>()] = new int[2] { ModContent.TileType<PyreMantle>(), ModContent.TileType<PyreMantleMolten>() };

            VineToGrass = new Dictionary<ushort, ushort>
            {
                [TileID.Vines] = TileID.Grass,
                [TileID.Vines] = TileID.LeafBlock,
                [TileID.CrimsonVines] = TileID.CrimsonGrass,
                [TileID.HallowedVines] = TileID.HallowedGrass,
                [(ushort)ModContent.TileType<AstralVines>()] = (ushort)ModContent.TileType<AstralGrass>()
            };

            tileMergeTypes = new bool[TileLoader.TileCount][];
            for (var i = 0; i < tileMergeTypes.Length; ++i)
                tileMergeTypes[i] = new bool[TileLoader.TileCount];
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralOre>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralStone>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralSand>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralSnow>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<AstralClay>()] = true;
            tileMergeTypes[ModContent.TileType<AstralDirt>()][ModContent.TileType<NovaeSlag>()] = true;
            tileMergeTypes[ModContent.TileType<AstralSnow>()][ModContent.TileType<AstralIce>()] = true;
            tileMergeTypes[ModContent.TileType<AstralSand>()][ModContent.TileType<HardenedAstralSand>()] = true;
            tileMergeTypes[ModContent.TileType<HardenedAstralSand>()][ModContent.TileType<AstralSandstone>()] = true;
            tileMergeTypes[ModContent.TileType<AstralSandstone>()][ModContent.TileType<CelestialRemains>()] = true;

            tileMergeTypes[ModContent.TileType<BrimstoneSlag>()][ModContent.TileType<InfernalSuevite>()] = true;

            tileMergeTypes[TileID.Sandstone][ModContent.TileType<EutrophicSand>()] = true;
            tileMergeTypes[ModContent.TileType<EutrophicSand>()][ModContent.TileType<Navystone>()] = true;
            tileMergeTypes[ModContent.TileType<Navystone>()][ModContent.TileType<SeaPrism>()] = true;

            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<ScoriaOre>()] = true;
            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<PlantyMush>()] = true;
            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<Voidstone>()] = true;
            tileMergeTypes[ModContent.TileType<AbyssGravel>()][ModContent.TileType<SulphurousSandstone>()] = true;
            tileMergeTypes[ModContent.TileType<SulphurousSandstone>()][ModContent.TileType<SulphurousSand>()] = true;
        }

        public override void Unload()
        {
            PlantValidGrounds = null;

            VineToGrass?.Clear();
            VineToGrass = null;

            tileMergeTypes = null;
        }
        #endregion

        #region Tile Variation Helpers
        public static int GetVariation4x4_012_Low0(int i, int j)
        {
            int xRel = i & 0b0011;
            int yRel = j & 0b0011;
            var output = xRel switch
            {
                0 => (yRel switch
                {
                    0 => 0,
                    1 => 2,
                    2 => 1,
                    _ => 2
                }),
                1 => (yRel switch
                {
                    0 => 2,
                    1 => 0,
                    2 => 2,
                    _ => 2
                }),
                2 => (yRel switch
                {
                    0 => 2,
                    1 => 0,
                    2 => 1,
                    _ => 2
                }),
                _ => (yRel switch
                {
                    0 => 1,
                    1 => 2,
                    2 => 0,
                    _ => 2
                }),
            };
            return output;
        }

        public static int GetVariation4x4_01_Low0(int i, int j)
        {
            int xRel = i & 0b0011;
            int yRel = j & 0b0011;
            var output = xRel switch
            {
                0 => (yRel switch
                {
                    0 => 0,
                    1 => 0,
                    2 => 1,
                    _ => 1
                }),
                1 => (yRel switch
                {
                    0 => 1,
                    1 => 0,
                    2 => 1,
                    _ => 1
                }),
                2 => (yRel switch
                {
                    0 => 1,
                    1 => 0,
                    2 => 0,
                    _ => 1
                }),
                _ => (yRel switch
                {
                    0 => 0,
                    1 => 1,
                    2 => 0,
                    _ => 1
                }),
            };
            return output;
        }

        public static int GetVariation3x3_01234_Low3(int i, int j)
        {
            int xRel = i % 3;
            int yRel = j % 3;
            var output = xRel switch
            {
                0 => (yRel switch
                {
                    0 => 0,
                    1 => 1,
                    _ => 2
                }),
                1 => (yRel switch
                {
                    0 => 2,
                    1 => 3,
                    _ => 4
                }),
                _ => (yRel switch
                {
                    0 => 4,
                    1 => 0,
                    _ => 1
                }),
            };
            return output;
        }
        #endregion

        #region Framing Helpers
        private static bool GetMerge(Tile myTile, Tile mergeTile)
        {
            if (!mergeTile.HasTile)
                return false;
            int myTileID = myTile.TileType;
            int otherTileID = mergeTile.TileType;
            return myTileID == otherTileID || Main.tileMerge[myTileID][otherTileID];
        }

        private static bool GetBlendSpecific(Tile myTile, Tile mergeTile, int blendType, bool includeSame)
        {
            if (!mergeTile.HasTile)
                return false;
            int myTileID = myTile.TileType;
            int otherTileID = mergeTile.TileType;
            return otherTileID == blendType || includeSame && myTileID == otherTileID;
        }

        private static void GetAdjacentTiles(int x, int y, out bool up, out bool down, out bool left, out bool right, out bool upLeft, out bool upRight, out bool downLeft, out bool downRight)
        {
            // These all get null checked in the GetMerge function
            var tile = Main.tile[x, y];
            var north = Main.tile[x, y - 1];
            var south = Main.tile[x, y + 1];
            var west = Main.tile[x - 1, y];
            var east = Main.tile[x + 1, y];
            var southwest = Main.tile[x - 1, y + 1];
            var southeast = Main.tile[x + 1, y + 1];
            var northwest = Main.tile[x - 1, y - 1];
            var northeast = Main.tile[x + 1, y - 1];

            left = false;
            right = false;
            up = false;
            down = false;
            upLeft = false;
            upRight = false;
            downLeft = false;
            downRight = false;

            if (GetMerge(tile, north) && (north.Slope == 0 || north.Slope == SlopeType.SlopeDownLeft || north.Slope == SlopeType.SlopeDownRight))
                up = true;
            if (GetMerge(tile, south) && (south.Slope == 0 || south.Slope == SlopeType.SlopeUpLeft || south.Slope == SlopeType.SlopeUpRight))
                down = true;
            if (GetMerge(tile, west) && (west.Slope == 0 || west.Slope == SlopeType.SlopeDownRight || west.Slope == SlopeType.SlopeUpRight))
                left = true;
            if (GetMerge(tile, east) && (east.Slope == 0 || east.Slope == SlopeType.SlopeDownLeft || east.Slope == SlopeType.SlopeUpLeft))
                right = true;
            if (GetMerge(tile, north) && GetMerge(tile, west) && GetMerge(tile, northwest) && (northwest.Slope == 0 || northwest.Slope == SlopeType.SlopeDownRight) && (north.Slope == 0 || north.Slope == SlopeType.SlopeDownLeft || north.Slope == SlopeType.SlopeUpLeft) && (west.Slope == 0 || west.Slope == SlopeType.SlopeUpLeft || west.Slope == SlopeType.SlopeUpRight))
                upLeft = true;
            if (GetMerge(tile, north) && GetMerge(tile, east) && GetMerge(tile, northeast) && (northeast.Slope == 0 || northeast.Slope == SlopeType.SlopeDownLeft) && (north.Slope == 0 || north.Slope == SlopeType.SlopeDownRight || north.Slope == SlopeType.SlopeUpRight) && (east.Slope == 0 || east.Slope == SlopeType.SlopeUpLeft || east.Slope == SlopeType.SlopeUpRight))
                upRight = true;
            if (GetMerge(tile, south) && GetMerge(tile, west) && GetMerge(tile, southwest) && !southwest.IsHalfBlock && (southwest.Slope == 0 || southwest.Slope == SlopeType.SlopeUpRight) && (south.Slope == 0 || south.Slope == SlopeType.SlopeDownLeft || south.Slope == SlopeType.SlopeUpLeft) && (west.Slope == 0 || west.Slope == SlopeType.SlopeDownLeft || west.Slope == SlopeType.SlopeDownRight))
                downLeft = true;
            if (GetMerge(tile, south) && GetMerge(tile, east) && GetMerge(tile, southeast) && !southeast.IsHalfBlock && (southeast.Slope == 0 || southeast.Slope == SlopeType.SlopeUpLeft) && (south.Slope == 0 || south.Slope == SlopeType.SlopeDownRight || south.Slope == SlopeType.SlopeUpRight) && (east.Slope == 0 || east.Slope == SlopeType.SlopeDownLeft || east.Slope == SlopeType.SlopeDownRight))
                downRight = true;
        }

        private static void SetFrameAt(int x, int y, int frameX, int frameY)
        {
            var tile = Main.tile[x, y];
            if (tile != null)
            {
                tile.TileFrameX = (short)frameX;
                tile.TileFrameY = (short)frameY;
            }
        }
        #endregion

        #region Specific Framing Code
        internal static void PlantFrame(int x, int y)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return;

            if (y < 0 || y >= Main.maxTilesY)
                return;

            // If the tile below is off the bottom of the map, then assume it's invalid placement
            var tile = Main.tile[x, y];
            int plantType = tile.TileType;
            if (y + 1 >= Main.maxTilesY)
            {
                WorldGen.KillTile(x, y);
                return;
            }

            // If tile below is not elligible for growing plants, we kill the tile immediately
            var below = Main.tile[x, y + 1];
            if (!below.HasTile || !below.HasUnactuatedTile || below.IsHalfBlock || below.Slope != SlopeType.Solid)
            {
                WorldGen.KillTile(x, y);
                return;
            }

            // Check if tile below is valid for given grass type, If so we don't need to update this
            var belowTileType = (int)below.TileType;
            if (PlantValidGrounds[plantType] is not null && PlantValidGrounds[plantType].Contains(belowTileType))
                return;

            var newPlantType = plantType;

            if ((plantType == TileID.Plants || plantType == TileID.Plants2) && belowTileType != TileID.Grass && tile.TileFrameX >= 162)
            {
                Main.tile[x, y].TileFrameX = 126;
            }
            if (plantType == TileID.JunglePlants2 && belowTileType != TileID.JungleGrass && tile.TileFrameX >= 162)
            {
                Main.tile[x, y].TileFrameX = 126;
            }

            #region Biome Grass Replacements
            if (belowTileType == TileID.CorruptGrass)
            {
                newPlantType = TileID.CorruptPlants;
                if (tile.TileFrameX >= 162)
                {
                    Main.tile[x, y].TileFrameX = 126;
                }
            }
            else if (belowTileType == TileID.Grass)
            {
                newPlantType = (plantType == TileID.HallowedPlants2 ? TileID.Plants2 : TileID.Plants);
            }
            else if (belowTileType == TileID.HallowedGrass)
            {
                newPlantType = (plantType == TileID.Plants2 ? TileID.HallowedPlants2 : TileID.HallowedPlants);
            }
            else if (belowTileType == TileID.CrimsonGrass)
            {
                newPlantType = TileID.CrimsonPlants;
            }
            else if (belowTileType == TileID.MushroomGrass)
            {
                newPlantType = TileID.MushroomPlants;
                while (Main.tile[x, y].TileFrameX > 72)
                {
                    Main.tile[x, y].TileFrameX -= 72;
                }
            }
            else if (belowTileType == ModContent.TileType<AstralGrass>())
            {
                var isShortPlant = plantType == TileID.Plants ||
                    plantType == TileID.CorruptPlants ||
                    plantType == TileID.CrimsonPlants ||
                    plantType == TileID.HallowedPlants ||
                    plantType == TileID.MushroomPlants ||
                    plantType == TileID.JunglePlants;
                newPlantType = isShortPlant ? ModContent.TileType<AstralShortPlants>() : ModContent.TileType<AstralTallPlants>();
            }
            #endregion

            // If the tile type is not the same as the plant type, then set it equal. Otherwise, destroy it.
            if (plantType != newPlantType)
            {
                Main.tile[x, y].TileType = (ushort)newPlantType;
            }
        }

        internal static void VineFrame(int x, int y)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return;
            if (y < 0 || y >= Main.maxTilesY)
                return;

            var tile = Main.tile[x, y];
            int myType = tile.TileType;

            // Get the type of the tile above this vine. If that tile doesn't exist, just assume it's another vine.
            var north = y <= 0 ? default : Main.tile[x, y - 1];
            var northType = north == default(Tile) ? myType : !north.HasTile || north.BottomSlope ? -1 : north.TileType;

            // Make this vine match the tile above it if that's another vine or a grass tile.
            var vines = VineToGrass.Keys.ToArray();
            for (var i = 0; i < vines.Length; ++i)
            {
                var correspondingGrass = VineToGrass[vines[i]];
                if (myType != vines[i] && (northType == correspondingGrass || northType == vines[i]))
                {
                    Main.tile[x, y].TileType = vines[i];
                    WorldGen.SquareTileFrame(x, y, true);
                    return;
                }
            }

            // If the tile above is an identical vine, nothing else needs to be done.
            if (northType == myType)
                return;

            // If the tile above isn't sloped correctly or otherwise isn't a valid anchor for this vine, check whether the vine must die.
            var tileMustDie = northType == -1;
            if (northType != -1)
            {
                // Vanilla vines can hang from vanilla grass and vanilla leaf blocks.
                if (myType == TileID.Vines && northType != TileID.Grass && northType != TileID.LeafBlock)
                {
                    tileMustDie = true;
                }
                else if (myType != TileID.Vines)
                {
                    for (var i = 0; i < vines.Length; ++i)
                    {
                        // Not matching grass? Die.
                        if (myType == vines[i] && northType != VineToGrass[vines[i]])
                        {
                            tileMustDie = true;
                            break;
                        }
                    }
                }
            }

            if (tileMustDie)
                WorldGen.KillTile(x, y, false, false, false);
        }

        internal static bool BetterGemsparkFraming(int x, int y, bool resetFrame)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return false;
            if (y < 0 || y >= Main.maxTilesY)
                return false;

            var tile = Main.tile[x, y];
            if (tile.Slope > 0 && TileID.Sets.HasSlopeFrames[tile.TileType])
            {
                return true;
            }

            GetAdjacentTiles(x, y, out var up, out var down, out var left, out var right, out var upLeft, out var upRight, out var downLeft, out var downRight);

            // Reset the tile's random frame style if the frame is being reset.
            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].Get<TileWallWireStateData>().TileFrameNumber = randomFrame;
            }
            else
            {
                randomFrame = Main.tile[x, y].TileFrameNumber;
            }

            return GemsparkAndBrimstoneFraming(tile, randomFrame, false, up, down, left, right, upLeft, upRight, downLeft, downRight);
        }

        internal static bool BrimstoneFraming(int x, int y, bool resetFrame)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return false;
            if (y < 0 || y >= Main.maxTilesY)
                return false;

            var tile = Main.tile[x, y];
            if (tile.Slope > 0 && TileID.Sets.HasSlopeFrames[tile.TileType])
                return true;

            GetAdjacentTiles(x, y, out var up, out var down, out var left, out var right, out var upLeft, out var upRight, out var downLeft, out var downRight);

            // Reset the tile's random frame style if the frame is being reset.
            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].Get<TileWallWireStateData>().TileFrameNumber = randomFrame;
            }
            else
                randomFrame = Main.tile[x, y].TileFrameNumber;
            return GemsparkAndBrimstoneFraming(tile, randomFrame, true, up, down, left, right, upLeft, upRight, downLeft, downRight);
        }

        private static bool GemsparkAndBrimstoneFraming(
            Tile tile, int randomFrame, bool brimstone,
            bool up, bool down, bool left, bool right,
            bool upLeft, bool upRight, bool downLeft, bool downRight)
        {
            var randomFrameX54 = randomFrame * 54;

            /*
                8 2 9
                4 - 5
                6 3 7
            */
            #region L States
            if (!up && down && !left && right && !downRight)
            {
                tile.TileFrameX = (short)(brimstone ? (16 * 18 + randomFrameX54) : (13 * 18));
                tile.TileFrameY = 0;
                return false;
            }
            if (!up && down && left && !right && !downLeft)
            {
                tile.TileFrameX = (short)(brimstone ? (18 * 18 + randomFrameX54) : (15 * 18));
                tile.TileFrameY = 0;
                return false;
            }
            if (up && !down && !left && right && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (16 * 18 + randomFrameX54) : (13 * 18));
                tile.TileFrameY = 2 * 18;
                return false;
            }
            if (up && !down && left && !right && !upLeft)
            {
                tile.TileFrameX = (short)(brimstone ? (18 * 18 + randomFrameX54) : (15 * 18));
                tile.TileFrameY = 2 * 18;
                return false;
            }
            #endregion

            #region T States
            if (!up && down && left && right && !downLeft && !downRight)
            {
                tile.TileFrameX = (short)(brimstone ? (17 * 18 + randomFrameX54) : (14 * 18));
                tile.TileFrameY = 0;
                return false;
            }
            if (up && !down && left && right && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (17 * 18 + randomFrameX54) : (14 * 18));
                tile.TileFrameY = 2 * 18;
                return false;
            }
            if (up && down && !left && right && !downRight && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (16 * 18 + randomFrameX54) : (13 * 18));
                tile.TileFrameY = 18;
                return false;
            }
            if (up && down && left && !right && !downLeft && !upLeft)
            {
                tile.TileFrameX = (short)(brimstone ? (18 * 18 + randomFrameX54) : (15 * 18));
                tile.TileFrameY = 18;
                return false;
            }
            #endregion

            #region X State
            if (up && down && left && right && !downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (17 * 18 + randomFrameX54) : (14 * 18));
                tile.TileFrameY = 18;
                return false;
            }
            #endregion

            #region Inner Corner x1
            if (up && down && left && right && !downLeft && downRight && upLeft && upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (14 * 18) : (15 * 18));
                tile.TileFrameY = (short)(brimstone ? (5 * 18 + randomFrame * 36) : (3 * 18));
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (13 * 18) : (14 * 18));
                tile.TileFrameY = (short)(brimstone ? (5 * 18 + randomFrame * 36) : (3 * 18));
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (14 * 18) : (15 * 18));
                tile.TileFrameY = (short)(brimstone ? (6 * 18 + randomFrame * 36) : (4 * 18));
                return false;
            }
            if (up && down && left && right && downLeft && downRight && upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (13 * 18) : (14 * 18));
                tile.TileFrameY = (short)(brimstone ? (6 * 18 + randomFrame * 36) : (4 * 18));
                return false;
            }
            #endregion

            #region Inner Corner x2 (same side)
            if (up && down && left && right && !downLeft && !downRight && upLeft && upRight)
            {
                tile.TileFrameX = (short)(6 * 18 + randomFrame * 18);
                tile.TileFrameY = 2 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)(6 * 18 + randomFrame * 18);
                tile.TileFrameY = 1 * 18;
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 10 * 18;
                tile.TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 11 * 18;
                tile.TileFrameY = (short)(randomFrame * 18);
                return false;
            }
            #endregion

            #region Inner Corner x2 (opposite corners)
            if (up && down && left && right && !downLeft && downRight && upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (10 * 18 + randomFrame * 18) : (16 * 18));
                tile.TileFrameY = 4 * 18;
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (13 * 18 + randomFrame * 18) : (17 * 18));
                tile.TileFrameY = 4 * 18;
                return false;
            }
            #endregion

            #region Inner Corner x3
            if (up && down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (15 * 18) : (12 * 18));
                tile.TileFrameY = (short)(brimstone ? (6 * 18 + randomFrame * 36) : (4 * 18));
                return false;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (15 * 18) : (12 * 18));
                tile.TileFrameY = (short)(brimstone ? (5 * 18 + randomFrame * 36) : (3 * 18));
                return false;
            }
            if (up && down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (16 * 18) : (13 * 18));
                tile.TileFrameY = (short)(brimstone ? (6 * 18 + randomFrame * 36) : (4 * 18));
                return false;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (16 * 18) : (13 * 18));
                tile.TileFrameY = (short)(brimstone ? (5 * 18 + randomFrame * 36) : (3 * 18));
                return false;
            }
            #endregion

            #region Corner and Side
            if (!up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (17 * 18 + randomFrame * 36) : (17 * 18));
                tile.TileFrameY = (short)(brimstone ? (3 * 18) : (2 * 18));
                return false;
            }
            if (!up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (16 * 18 + randomFrame * 36) : (16 * 18));
                tile.TileFrameY = (short)(brimstone ? (3 * 18) : (2 * 18));
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (17 * 18 + randomFrame * 36) : (17 * 18));
                tile.TileFrameY = (short)(brimstone ? (4 * 18) : (3 * 18));
                return false;
            }
            if (up && !down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (16 * 18 + randomFrame * 36) : (16 * 18));
                tile.TileFrameY = (short)(brimstone ? (4 * 18) : (3 * 18));
                return false;
            }
            if (up && down && !left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (17 * 18) : (16 * 18));
                tile.TileFrameY = (short)(brimstone ? (5 * 18 + randomFrame * 36) : (0));
                return false;
            }
            if (up && down && !left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (17 * 18) : (16 * 18));
                tile.TileFrameY = (short)(brimstone ? (6 * 18 + randomFrame * 36) : (18));
                return false;
            }
            if (up && down && left && !right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (18 * 18) : (17 * 18));
                tile.TileFrameY = (short)(brimstone ? (5 * 18 + randomFrame * 36) : (0));
                return false;
            }
            if (up && down && left && !right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = (short)(brimstone ? (18 * 18) : (17 * 18));
                tile.TileFrameY = (short)(brimstone ? (6 * 18 + randomFrame * 36) : (18));
                return false;
            }
            #endregion

            return true;
        }

        internal static void CompactFraming(int x, int y, bool resetFrame = true)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return;
            if (y < 0 || y >= Main.maxTilesY)
                return;

            var tile = Main.tile[x, y];
            if (tile.Slope > 0 && TileID.Sets.HasSlopeFrames[tile.TileType])
                return;

            // Reset the tile's random frame style if the frame is being reset.
            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].Get<TileWallWireStateData>().TileFrameNumber = (byte)randomFrame;
            }
            else
                randomFrame = Main.tile[x, y].TileFrameNumber;

            GetAdjacentTiles(x, y, out var up, out var down, out var left, out var right, out var upLeft, out var upRight, out var downLeft, out var downRight);

            #region Middle State
            if (up && down && left && right && upLeft && upRight && downLeft && downRight)
            {
                tile.TileFrameX = 18;
                tile.TileFrameY = 18;
                return;
            }
            #endregion

            #region Single State
            if (!up && !down && !left && !right)
            {
                tile.TileFrameX = 54;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region Edges
            if (!up && down && left && right && downLeft && downRight)
            {
                tile.TileFrameX = 18;
                tile.TileFrameY = 0;
                return;
            }
            if (up && down && !left && right && upRight && downRight)
            {
                tile.TileFrameX = 0;
                tile.TileFrameY = 18;
                return;
            }
            if (up && !down && left && right && upLeft && upRight)
            {
                tile.TileFrameX = 18;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && left && !right && upLeft && downLeft)
            {
                tile.TileFrameX = 36;
                tile.TileFrameY = 18;
                return;
            }
            #endregion

            #region Edge Corners
            if (!up && down && !left && right && downRight)
            {
                tile.TileFrameX = 0;
                tile.TileFrameY = 0;
                return;
            }
            if (!up && down && left && !right && downLeft)
            {
                tile.TileFrameX = 36;
                tile.TileFrameY = 0;
                return;
            }
            if (up && !down && !left && right && upRight)
            {
                tile.TileFrameX = 0;
                tile.TileFrameY = 36;
                return;
            }
            if (up && !down && left && !right && upLeft)
            {
                tile.TileFrameX = 36;
                tile.TileFrameY = 36;
                return;
            }
            #endregion

            #region I States
            if (up && down && !left && !right)
            {
                tile.TileFrameX = 54;
                tile.TileFrameY = 18;
                return;
            }
            if (!up && !down && left && right)
            {
                tile.TileFrameX = 18;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region I End States
            if (!up && down && !left && !right)
            {
                tile.TileFrameX = 54;
                tile.TileFrameY = 0;
                return;
            }
            if (up && !down && !left && !right)
            {
                tile.TileFrameX = 54;
                tile.TileFrameY = 36;
                return;
            }
            if (!up && !down && !left && right)
            {
                tile.TileFrameX = 0;
                tile.TileFrameY = 54;
                return;
            }
            if (!up && !down && left && !right)
            {
                tile.TileFrameX = 36;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region L States
            if (!up && down && !left && right && !downRight)
            {
                tile.TileFrameX = 72;
                tile.TileFrameY = 0;
                return;
            }
            if (!up && down && left && !right && !downLeft)
            {
                tile.TileFrameX = 108;
                tile.TileFrameY = 0;
                return;
            }
            if (up && !down && !left && right && !upRight)
            {
                tile.TileFrameX = 72;
                tile.TileFrameY = 36;
                return;
            }
            if (up && !down && left && !right && !upLeft)
            {
                tile.TileFrameX = 108;
                tile.TileFrameY = 36;
                return;
            }
            #endregion

            #region T States
            if (!up && down && left && right && !downLeft && !downRight)
            {
                tile.TileFrameX = 90;
                tile.TileFrameY = 0;
                return;
            }
            if (up && !down && left && right && !upLeft && !upRight)
            {
                tile.TileFrameX = 90;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && !left && right && !downRight && !upRight)
            {
                tile.TileFrameX = 72;
                tile.TileFrameY = 18;
                return;
            }
            if (up && down && left && !right && !downLeft && !upLeft)
            {
                tile.TileFrameX = 108;
                tile.TileFrameY = 18;
                return;
            }
            #endregion

            #region X State
            if (up && down && left && right && !downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 90;
                tile.TileFrameY = 18;
                return;
            }
            #endregion

            #region Inner Corner x1
            if (up && down && left && right && !downLeft && downRight && upLeft && upRight)
            {
                tile.TileFrameX = 144;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && upRight)
            {
                tile.TileFrameX = 126;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 144;
                tile.TileFrameY = 54;
                return;
            }
            if (up && down && left && right && downLeft && downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 126;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region Inner Corner x2 (same side)
            if (up && down && left && right && !downLeft && !downRight && upLeft && upRight)
            {
                tile.TileFrameX = 198;
                tile.TileFrameY = 0;
                return;
            }
            if (up && down && left && right && downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 198;
                tile.TileFrameY = 18;
                return;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 198;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && left && right && downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 198;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region Inner Corner x2 (opposite corners)
            if (up && down && left && right && !downLeft && downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 108;
                tile.TileFrameY = 54;
                return;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 90;
                tile.TileFrameY = 54;
                return;
            }
            #endregion

            #region Inner Corner x3
            if (up && down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 126;
                tile.TileFrameY = 18;
                return;
            }
            if (up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 126;
                tile.TileFrameY = 0;
                return;
            }
            if (up && down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 144;
                tile.TileFrameY = 18;
                return;
            }
            if (up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 144;
                tile.TileFrameY = 0;
                return;
            }
            #endregion

            #region Corner and Side
            if (!up && down && left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 180;
                tile.TileFrameY = 0;
                return;
            }
            if (!up && down && left && right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 162;
                tile.TileFrameY = 0;
                return;
            }
            if (up && !down && left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 180;
                tile.TileFrameY = 18;
                return;
            }
            if (up && !down && left && right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 162;
                tile.TileFrameY = 18;
                return;
            }
            if (up && down && !left && right && !downLeft && !downRight && !upLeft && upRight)
            {
                tile.TileFrameX = 162;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && !left && right && !downLeft && downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 162;
                tile.TileFrameY = 54;
                return;
            }
            if (up && down && left && !right && !downLeft && !downRight && upLeft && !upRight)
            {
                tile.TileFrameX = 180;
                tile.TileFrameY = 36;
                return;
            }
            if (up && down && left && !right && downLeft && !downRight && !upLeft && !upRight)
            {
                tile.TileFrameX = 180;
                tile.TileFrameY = 54;
                return;
            }
            #endregion
        }

        internal static void SlopedGlowmask(ref readonly Tile tile, int i, int j, Texture2D texture, Rectangle? sourceRectangle, Color drawColor, Vector2 positionOffset)
        {
            int frameX = tile.TileFrameX;
            int frameY = tile.TileFrameY;

            int width = 16;
            int height = 16;

            if (sourceRectangle != null)
            {
                frameX = ((Rectangle)sourceRectangle).X;
                frameY = ((Rectangle)sourceRectangle).Y;
            }

            int iX16 = i * 16;
            int jX16 = j * 16;

            Vector2 location = new Vector2(iX16, jX16);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            Vector2 offsets = -Main.screenPosition + zero + positionOffset;
            Vector2 drawCoordinates = location + offsets;

            if ((tile.Slope == 0 && !tile.IsHalfBlock) || (Main.tileSolid[tile.TileType] && Main.tileSolidTop[tile.TileType])) //second one should be for platforms
            {
                Main.spriteBatch.Draw(texture, drawCoordinates, new Rectangle(frameX, frameY, width, height), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else if (tile.IsHalfBlock)
            {
                Main.spriteBatch.Draw(texture, new Vector2(drawCoordinates.X, drawCoordinates.Y + 8), new Rectangle(frameX, frameY, width, 8), drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                byte b = (byte)tile.Slope;
                Rectangle TileFrame;
                Vector2 drawPos;
                if (b == 1 || b == 2)
                {
                    int length;
                    int height2;
                    for (int a = 0; a < 8; ++a)
                    {
                        int aX2 = a * 2;
                        if (b == 2)
                        {
                            length = 16 - aX2 - 2;
                            height2 = 14 - aX2;
                        }
                        else
                        {
                            length = aX2;
                            height2 = 14 - length;
                        }

                        TileFrame = new Rectangle(frameX + length, frameY, 2, height2);
                        drawPos = new Vector2(iX16 + length, jX16 + aX2) + offsets;
                        Main.spriteBatch.Draw(texture, drawPos, TileFrame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    }

                    TileFrame = new Rectangle(frameX, frameY + 14, 16, 2);
                    drawPos = new Vector2(iX16, jX16 + 14) + offsets;
                    Main.spriteBatch.Draw(texture, drawPos, TileFrame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                }
                else
                {
                    int length;
                    int height2;
                    for (int a = 0; a < 8; ++a)
                    {
                        int aX2 = a * 2;
                        if (b == 3)
                        {
                            length = aX2;
                            height2 = 16 - length;
                        }
                        else
                        {
                            length = 16 - aX2 - 2;
                            height2 = 16 - aX2;
                        }

                        TileFrame = new Rectangle(frameX + length, frameY + 16 - height2, 2, height2);
                        drawPos = new Vector2(iX16 + length, jX16) + offsets;
                        Main.spriteBatch.Draw(texture, drawPos, TileFrame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    }

                    drawPos = new Vector2(iX16, jX16) + offsets;
                    if (tile.TileType != EutrophicGlass.TypeCache)
                    {
                        TileFrame = new Rectangle(frameX, frameY, 16, 2);
                        Main.spriteBatch.Draw(texture, drawPos, TileFrame, drawColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
                    }
                }
            }
            // Contribuited by Vortex
        }
        #endregion

        #region Generic Custom Framing Code

        internal static void CustomMergeFrameExplicit(int x, int y, int myType, int mergeType, out bool mergedUp,
            out bool mergedLeft, out bool mergedRight, out bool mergedDown, bool forceSameDown = false,
            bool forceSameUp = false, bool forceSameLeft = false, bool forceSameRight = false, bool resetFrame = true, bool myTypeBrimFrame = false)
        {
            if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
            {
                mergedUp = mergedLeft = mergedRight = mergedDown = false;
                return;
            }

            // Disable vanilla trying to merge these tiles automtaically.
            Main.tileMerge[myType][mergeType] = false;

            // These all get null checked in the GetSimilarity and GetMerge functions
            var tileLeft = Main.tile[x - 1, y];
            var tileRight = Main.tile[x + 1, y];
            var tileUp = Main.tile[x, y - 1];
            var tileDown = Main.tile[x, y + 1];
            var tileTopLeft = Main.tile[x - 1, y - 1];
            var tileTopRight = Main.tile[x + 1, y - 1];
            var tileBottomLeft = Main.tile[x - 1, y + 1];
            var tileBottomRight = Main.tile[x + 1, y + 1];

            // Cardinal directions
            var leftSim = forceSameLeft ? Similarity.Same : GetSimilarity(tileLeft, myType, mergeType);
            var rightSim = forceSameRight ? Similarity.Same : GetSimilarity(tileRight, myType, mergeType);
            var upSim = forceSameUp ? Similarity.Same : GetSimilarity(tileUp, myType, mergeType);
            var downSim = forceSameDown ? Similarity.Same : GetSimilarity(tileDown, myType, mergeType);

            // Diagonal directions
            var topLeftSim = GetSimilarity(tileTopLeft, myType, mergeType);
            var topRightSim = GetSimilarity(tileTopRight, myType, mergeType);
            var bottomLeftSim = GetSimilarity(tileBottomLeft, myType, mergeType);
            var bottomRightSim = GetSimilarity(tileBottomRight, myType, mergeType);

            // Reset the tile's random frame style if the frame is being reset.
            int randomFrame;
            if (resetFrame)
            {
                randomFrame = WorldGen.genRand.Next(3);
                Main.tile[x, y].Get<TileWallWireStateData>().TileFrameNumber = (byte)randomFrame;
            }
            else
            {
                randomFrame = Main.tile[x, y].TileFrameNumber;
            }

            // Initialize all merged variables to false.
            mergedDown = mergedLeft = mergedRight = mergedUp = false;

            #region Custom Merge Conditional Tree
            if (leftSim == Similarity.None)
            {
                if (upSim == Similarity.Same)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            SetFrameAt(x, y, 0, 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            mergedRight = true;
                            SetFrameAt(x, y, 234 + 18 * randomFrame, 36);
                            return;
                        }
                        SetFrameAt(x, y, 90, 18 * randomFrame);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedDown = true;
                            SetFrameAt(x, y, 72, 90 + 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                            return;
                        }
                        mergedDown = true;
                        SetFrameAt(x, y, 126, 90 + 18 * randomFrame);
                        return;
                    }
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 36 * randomFrame, 72);
                        return;
                    }
                    SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                    return;
                }
                else if (upSim == Similarity.MergeLink)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedUp = true;
                            SetFrameAt(x, y, 72, 144 + 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                            return;
                        }
                        mergedUp = true;
                        SetFrameAt(x, y, 126, 144 + 18 * randomFrame);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            SetFrameAt(x, y, 162, 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                            return;
                        }
                        mergedUp = true;
                        mergedDown = true;
                        SetFrameAt(x, y, 108, 216 + 18 * randomFrame);
                        return;
                    }
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    mergedUp = true;
                    SetFrameAt(x, y, 108, 144 + 18 * randomFrame);
                    return;
                }
                if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 36 * randomFrame, 54);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                        return;
                    }
                    SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                    return;
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    mergedDown = true;
                    SetFrameAt(x, y, 108, 90 + 18 * randomFrame);
                    return;
                }
                if (rightSim == Similarity.Same)
                {
                    SetFrameAt(x, y, 162, 18 * randomFrame);
                    return;
                }
                else if (rightSim == Similarity.MergeLink)
                {
                    mergedRight = true;
                    SetFrameAt(x, y, 54 + 18 * randomFrame, 234);
                    return;
                }
                SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                return;
            }
            else if (leftSim == Similarity.MergeLink)
            {
                if (upSim == Similarity.Same)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedLeft = true;
                            SetFrameAt(x, y, 162, 126 + 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            mergedLeft = true;
                            mergedRight = true;
                            SetFrameAt(x, y, 180, 126 + 18 * randomFrame);
                            return;
                        }
                        mergedLeft = true;
                        SetFrameAt(x, y, 234 + 18 * randomFrame, 54);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedLeft = mergedDown = true;
                            SetFrameAt(x, y, 36, 108 + 36 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            mergedLeft = mergedRight = mergedDown = true;
                            SetFrameAt(x, y, 198, 144 + 18 * randomFrame);
                            return;
                        }
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                        return;
                    }
                    if (rightSim == Similarity.Same)
                    {
                        mergedLeft = true;
                        SetFrameAt(x, y, 18 * randomFrame, 216);
                        return;
                    }
                    SetFrameAt(x, y, 108 + 18 * randomFrame, 54);
                    return;
                }
                else if (upSim == Similarity.MergeLink)
                {
                    if (downSim == Similarity.Same)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedUp = mergedLeft = true;
                            SetFrameAt(x, y, 36, 90 + 36 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            mergedLeft = mergedRight = mergedUp = true;
                            SetFrameAt(x, y, 198, 90 + 18 * randomFrame);
                            return;
                        }
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                        return;
                    }
                    else if (downSim == Similarity.MergeLink)
                    {
                        if (rightSim == Similarity.Same)
                        {
                            mergedUp = mergedLeft = mergedDown = true;
                            SetFrameAt(x, y, 216, 90 + 18 * randomFrame);
                            return;
                        }
                        else if (rightSim == Similarity.MergeLink)
                        {
                            mergedDown = mergedLeft = mergedRight = mergedUp = true;
                            SetFrameAt(x, y, 108 + 18 * randomFrame, 198);
                            return;
                        }
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                    return;
                }
                if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedLeft = true;
                        SetFrameAt(x, y, 18 * randomFrame, 198);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                        return;
                    }
                    SetFrameAt(x, y, 108 + 18 * randomFrame, 0);
                    return;
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        SetFrameAt(x, y, 162, 18 * randomFrame);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                        return;
                    }
                    SetFrameAt(x, y, 162 + 18 * randomFrame, 54);
                    return;
                }
                if (rightSim == Similarity.Same)
                {
                    mergedLeft = true;
                    SetFrameAt(x, y, 18 * randomFrame, 252);
                    return;
                }
                else if (rightSim == Similarity.MergeLink)
                {
                    mergedRight = mergedLeft = true;
                    SetFrameAt(x, y, 162 + 18 * randomFrame, 198);
                    return;
                }
                mergedLeft = true;
                SetFrameAt(x, y, 18 * randomFrame, 234);
                return;
            }
            if (upSim == Similarity.Same)
            {
                if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        #region FULL TILE STUFF
                        if (topLeftSim == Similarity.MergeLink || topRightSim == Similarity.MergeLink || bottomLeftSim == Similarity.MergeLink || bottomRightSim == Similarity.MergeLink)
                        {
                            if (bottomRightSim == Similarity.MergeLink)
                            {
                                SetFrameAt(x, y, 0, 90 + 36 * randomFrame);
                                return;
                            }
                            else if (bottomLeftSim == Similarity.MergeLink)
                            {
                                SetFrameAt(x, y, 18, 90 + 36 * randomFrame);
                                return;
                            }
                            else if (topRightSim == Similarity.MergeLink)
                            {
                                SetFrameAt(x, y, 0, 108 + 36 * randomFrame);
                                return;
                            }
                            SetFrameAt(x, y, 18, 108 + 36 * randomFrame);
                            return;
                        }
                        if (topLeftSim == Similarity.Same)
                        {
                            if (topRightSim == Similarity.Same)
                            {
                                if (bottomLeftSim == Similarity.Same)
                                {
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                if (bottomRightSim == Similarity.Same)
                                {
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                SetFrameAt(x, y, 108 + 18 * randomFrame, 36);
                                return;
                            }
                            if (bottomLeftSim == Similarity.Same)
                            {
                                if (bottomRightSim == Similarity.Same)
                                {
                                    if (topRightSim == Similarity.MergeLink)
                                    {
                                        SetFrameAt(x, y, 0, 108 + 36 * randomFrame);
                                        return;
                                    }
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                SetFrameAt(x, y, 198, 18 * randomFrame);
                                return;
                            }
                        }
                        else if (topLeftSim == Similarity.None)
                        {
                            if (topRightSim == Similarity.Same)
                            {
                                if (bottomRightSim == Similarity.Same)
                                {
                                    if (bottomLeftSim == Similarity.Same)
                                    {
                                        SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                        return;
                                    }
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                                if (bottomLeftSim == Similarity.Same)
                                {
                                    SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                                    return;
                                }
                            }
                            SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                            return;
                        }
                        SetFrameAt(x, y, 18 + 18 * randomFrame, 18);
                        return;
                        #endregion
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        mergedRight = true;
                        SetFrameAt(x, y, 144, 126 + 18 * randomFrame);
                        return;
                    }
                    SetFrameAt(x, y, 72, 18 * randomFrame);
                    return;
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedDown = true;
                        SetFrameAt(x, y, 144 + 18 * randomFrame, 90);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        mergedDown = mergedRight = true;
                        SetFrameAt(x, y, 54, 108 + 36 * randomFrame);
                        return;
                    }
                    mergedDown = true;
                    SetFrameAt(x, y, 90, 90 + 18 * randomFrame);
                    return;
                }
                if (rightSim == Similarity.Same)
                {
                    SetFrameAt(x, y, 18 + 18 * randomFrame, 36);
                    return;
                }
                else if (rightSim == Similarity.MergeLink)
                {
                    mergedRight = true;
                    SetFrameAt(x, y, 54 + 18 * randomFrame, 216);
                    return;
                }
                SetFrameAt(x, y, 18 + 36 * randomFrame, 72);
                return;
            }
            else if (upSim == Similarity.MergeLink)
            {
                if (downSim == Similarity.Same)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedUp = true;
                        SetFrameAt(x, y, 144 + 18 * randomFrame, 108);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        mergedRight = mergedUp = true;
                        SetFrameAt(x, y, 54, 90 + 36 * randomFrame);
                        return;
                    }
                    mergedUp = true;
                    SetFrameAt(x, y, 90, 144 + 18 * randomFrame);
                    return;
                }
                else if (downSim == Similarity.MergeLink)
                {
                    if (rightSim == Similarity.Same)
                    {
                        mergedUp = mergedDown = true;
                        SetFrameAt(x, y, 144 + 18 * randomFrame, 180);
                        return;
                    }
                    else if (rightSim == Similarity.MergeLink)
                    {
                        mergedUp = mergedRight = mergedDown = true;
                        SetFrameAt(x, y, 216, 144 + 18 * randomFrame);
                        return;
                    }
                    SetFrameAt(x, y, 216, 18 * randomFrame);
                    return;
                }
                if (rightSim == Similarity.Same)
                {
                    mergedUp = true;
                    SetFrameAt(x, y, 234 + 18 * randomFrame, 18);
                    return;
                }
                SetFrameAt(x, y, 216, 18 * randomFrame);
                return;
            }
            if (downSim == Similarity.Same)
            {
                if (rightSim == Similarity.Same)
                {
                    SetFrameAt(x, y, 18 + 18 * randomFrame, 0);
                    return;
                }
                else if (rightSim == Similarity.MergeLink)
                {
                    mergedRight = true;
                    SetFrameAt(x, y, 54 + 18 * randomFrame, 198);
                    return;
                }
                SetFrameAt(x, y, 18 + 36 * randomFrame, 54);
                return;
            }
            else if (downSim == Similarity.MergeLink)
            {
                if (rightSim == Similarity.Same)
                {
                    mergedDown = true;
                    SetFrameAt(x, y, 234 + 18 * randomFrame, 0);
                    return;
                }
                SetFrameAt(x, y, 216, 18 * randomFrame);
                return;
            }
            if (rightSim == Similarity.Same)
            {
                SetFrameAt(x, y, 108 + 18 * randomFrame, 72);
                return;
            }
            else if (rightSim == Similarity.MergeLink)
            {
                mergedRight = true;
                SetFrameAt(x, y, 54 + 18 * randomFrame, 252);
                return;
            }
            SetFrameAt(x, y, 216, 18 * randomFrame);
            return;
            #endregion
        }

        internal static void CustomMergeFrame(int x, int y, int myType, int mergeType, bool forceSameDown = false,
            bool forceSameUp = false, bool forceSameLeft = false, bool forceSameRight = false, bool resetFrame = true)
            => CustomMergeFrameExplicit(x, y, myType, mergeType, out _, out _, out _, out _, forceSameDown, forceSameUp, forceSameLeft, forceSameRight, resetFrame);

        internal static void CustomMergeFrame(int x, int y, int myType, int mergeType)
        {
            if (x < 0 || x >= Main.maxTilesX)
                return;
            if (y < 0 || y >= Main.maxTilesY)
                return;

            var forceSameUp = false;
            var forceSameDown = false;
            var forceSameLeft = false;
            var forceSameRight = false;

            var north = Main.tile[x, y - 1];
            var south = Main.tile[x, y + 1];
            var west = Main.tile[x - 1, y];
            var east = Main.tile[x + 1, y];

            if (north != null && north.HasTile && tileMergeTypes[myType][north.TileType])
            {
                // Register this tile as not automatically merging with the tile above it.
                CalamityUtils.SetMerge(myType, north.TileType, false);
                TileID.Sets.ChecksForMerge[myType] = true;

                // Properly frame the adjacent tile given this constraint.
                CustomMergeFrameExplicit(x, y - 1, north.TileType, myType, out _, out _, out _, out forceSameUp, false, false, false, false, false);
            }
            if (west != null && west.HasTile && tileMergeTypes[myType][west.TileType])
            {
                // Register this tile as not automatically merging with the tile to the left of it.
                CalamityUtils.SetMerge(myType, west.TileType, false);
                TileID.Sets.ChecksForMerge[myType] = true;

                // Properly frame the adjacent tile given this constraint.
                CustomMergeFrameExplicit(x - 1, y, west.TileType, myType, out _, out _, out forceSameLeft, out _, false, false, false, false, false);
            }
            if (east != null && east.HasTile && tileMergeTypes[myType][east.TileType])
            {
                // Register this tile as not automatically merging with the tile to the right of it.
                CalamityUtils.SetMerge(myType, east.TileType, false);
                TileID.Sets.ChecksForMerge[myType] = true;

                // Properly frame the adjacent tile given this constraint.
                CustomMergeFrameExplicit(x + 1, y, east.TileType, myType, out _, out forceSameRight, out _, out _, false, false, false, false, false);
            }
            if (south != null && south.HasTile && tileMergeTypes[myType][south.TileType])
            {
                // Register this tile as not automatically merging with the tile below it.
                CalamityUtils.SetMerge(myType, south.TileType, false);
                TileID.Sets.ChecksForMerge[myType] = true;

                // Properly frame the adjacent tile given this constraint.
                CustomMergeFrameExplicit(x, y + 1, south.TileType, myType, out forceSameDown, out _, out _, out _, false, false, false, false, false);
            }

            // With all constraints determined, properly frame the tile a final time.
            CustomMergeFrameExplicit(x, y, myType, mergeType, out _, out _, out _, out _, forceSameDown, forceSameUp, forceSameLeft, forceSameRight, true);
        }
        #endregion
    }
}

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
        // Rows are X, columns are Y, matching the original nested switches.
        private static readonly byte[,] Variation4x4_012 =
        {
            { 0, 2, 1, 2 },
            { 2, 0, 2, 2 },
            { 2, 0, 1, 2 },
            { 1, 2, 0, 2 }
        };

        private static readonly byte[,] Variation4x4_01 =
        {
            { 0, 0, 1, 1 },
            { 1, 0, 1, 1 },
            { 1, 0, 0, 1 },
            { 0, 1, 0, 1 }
        };

        private static readonly byte[,] Variation3x3_01234 =
        {
            { 0, 1, 2 },
            { 2, 3, 4 },
            { 4, 0, 1 }
        };

        public static int GetVariation4x4_012_Low0(int i, int j)
            => Variation4x4_012[i & 0b0011, j & 0b0011];

        public static int GetVariation4x4_01_Low0(int i, int j)
            => Variation4x4_01[i & 0b0011, j & 0b0011];

        public static int GetVariation3x3_01234_Low3(int i, int j)
            // Negative remainders originally selected the default (last) switch arm.
            => Variation3x3_01234[VariationIndex3(i), VariationIndex3(j)];

        private static int VariationIndex3(int coordinate)
            => coordinate % 3 is 0 or 1 ? coordinate % 3 : 2;
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
            => GemsparkAndBrimstoneFraming(x, y, resetFrame, false);

        internal static bool BrimstoneFraming(int x, int y, bool resetFrame)
            => GemsparkAndBrimstoneFraming(x, y, resetFrame, true);

        private static bool GemsparkAndBrimstoneFraming(int x, int y, bool resetFrame, bool brimstone)
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
            return GemsparkAndBrimstoneFraming(tile, randomFrame, brimstone, up, down, left, right, upLeft, upRight, downLeft, downRight);
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
            // Same adjacency order as CompactFraming; only the atlas coordinates differ.
            (int X, int Y)? frame = (up, down, left, right, upLeft, upRight, downLeft, downRight) switch
            {
                (false, true, false, true, _, _, _, false) => (brimstone ? 16 * 18 + randomFrameX54 : 13 * 18, 0),
                (false, true, true, false, _, _, false, _) => (brimstone ? 18 * 18 + randomFrameX54 : 15 * 18, 0),
                (true, false, false, true, _, false, _, _) => (brimstone ? 16 * 18 + randomFrameX54 : 13 * 18, 2 * 18),
                (true, false, true, false, false, _, _, _) => (brimstone ? 18 * 18 + randomFrameX54 : 15 * 18, 2 * 18),
                (false, true, true, true, _, _, false, false) => (brimstone ? 17 * 18 + randomFrameX54 : 14 * 18, 0),
                (true, false, true, true, false, false, _, _) => (brimstone ? 17 * 18 + randomFrameX54 : 14 * 18, 2 * 18),
                (true, true, false, true, _, false, _, false) => (brimstone ? 16 * 18 + randomFrameX54 : 13 * 18, 18),
                (true, true, true, false, false, _, false, _) => (brimstone ? 18 * 18 + randomFrameX54 : 15 * 18, 18),
                (true, true, true, true, false, false, false, false) => (brimstone ? 17 * 18 + randomFrameX54 : 14 * 18, 18),

                (true, true, true, true, true, true, false, true) => (brimstone ? 14 * 18 : 15 * 18, brimstone ? 5 * 18 + randomFrame * 36 : 3 * 18),
                (true, true, true, true, true, true, true, false) => (brimstone ? 13 * 18 : 14 * 18, brimstone ? 5 * 18 + randomFrame * 36 : 3 * 18),
                (true, true, true, true, false, true, true, true) => (brimstone ? 14 * 18 : 15 * 18, brimstone ? 6 * 18 + randomFrame * 36 : 4 * 18),
                (true, true, true, true, true, false, true, true) => (brimstone ? 13 * 18 : 14 * 18, brimstone ? 6 * 18 + randomFrame * 36 : 4 * 18),
                (true, true, true, true, true, true, false, false) => (6 * 18 + randomFrame * 18, 2 * 18),
                (true, true, true, true, false, false, true, true) => (6 * 18 + randomFrame * 18, 1 * 18),
                (true, true, true, true, false, true, false, true) => (10 * 18, randomFrame * 18),
                (true, true, true, true, true, false, true, false) => (11 * 18, randomFrame * 18),
                (true, true, true, true, true, false, false, true) => (brimstone ? 10 * 18 + randomFrame * 18 : 16 * 18, 4 * 18),
                (true, true, true, true, false, true, true, false) => (brimstone ? 13 * 18 + randomFrame * 18 : 17 * 18, 4 * 18),
                (true, true, true, true, false, true, false, false) => (brimstone ? 15 * 18 : 12 * 18, brimstone ? 6 * 18 + randomFrame * 36 : 4 * 18),
                (true, true, true, true, false, false, false, true) => (brimstone ? 15 * 18 : 12 * 18, brimstone ? 5 * 18 + randomFrame * 36 : 3 * 18),
                (true, true, true, true, true, false, false, false) => (brimstone ? 16 * 18 : 13 * 18, brimstone ? 6 * 18 + randomFrame * 36 : 4 * 18),
                (true, true, true, true, false, false, true, false) => (brimstone ? 16 * 18 : 13 * 18, brimstone ? 5 * 18 + randomFrame * 36 : 3 * 18),

                (false, true, true, true, false, false, false, true) => (brimstone ? 17 * 18 + randomFrame * 36 : 17 * 18, brimstone ? 3 * 18 : 2 * 18),
                (false, true, true, true, false, false, true, false) => (brimstone ? 16 * 18 + randomFrame * 36 : 16 * 18, brimstone ? 3 * 18 : 2 * 18),
                (true, false, true, true, false, true, false, false) => (brimstone ? 17 * 18 + randomFrame * 36 : 17 * 18, brimstone ? 4 * 18 : 3 * 18),
                (true, false, true, true, true, false, false, false) => (brimstone ? 16 * 18 + randomFrame * 36 : 16 * 18, brimstone ? 4 * 18 : 3 * 18),
                (true, true, false, true, false, true, false, false) => (brimstone ? 17 * 18 : 16 * 18, brimstone ? 5 * 18 + randomFrame * 36 : 0),
                (true, true, false, true, false, false, false, true) => (brimstone ? 17 * 18 : 16 * 18, brimstone ? 6 * 18 + randomFrame * 36 : 18),
                (true, true, true, false, true, false, false, false) => (brimstone ? 18 * 18 : 17 * 18, brimstone ? 5 * 18 + randomFrame * 36 : 0),
                (true, true, true, false, false, false, true, false) => (brimstone ? 18 * 18 : 17 * 18, brimstone ? 6 * 18 + randomFrame * 36 : 18),
                _ => null
            };
            return !ApplyFrame(tile, frame);
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

            // Ordered adjacency table: up, down, left, right, then the four diagonals.
            // A missing match leaves both existing frame coordinates untouched.
            (int X, int Y)? frame = (up, down, left, right, upLeft, upRight, downLeft, downRight) switch
            {
                (true, true, true, true, true, true, true, true) => (18, 18),
                (false, false, false, false, _, _, _, _) => (54, 54),
                (false, true, true, true, _, _, true, true) => (18, 0),
                (true, true, false, true, _, true, _, true) => (0, 18),
                (true, false, true, true, true, true, _, _) => (18, 36),
                (true, true, true, false, true, _, true, _) => (36, 18),
                (false, true, false, true, _, _, _, true) => (0, 0),
                (false, true, true, false, _, _, true, _) => (36, 0),
                (true, false, false, true, _, true, _, _) => (0, 36),
                (true, false, true, false, true, _, _, _) => (36, 36),
                (true, true, false, false, _, _, _, _) => (54, 18),
                (false, false, true, true, _, _, _, _) => (18, 54),
                (false, true, false, false, _, _, _, _) => (54, 0),
                (true, false, false, false, _, _, _, _) => (54, 36),
                (false, false, false, true, _, _, _, _) => (0, 54),
                (false, false, true, false, _, _, _, _) => (36, 54),
                (false, true, false, true, _, _, _, false) => (72, 0),
                (false, true, true, false, _, _, false, _) => (108, 0),
                (true, false, false, true, _, false, _, _) => (72, 36),
                (true, false, true, false, false, _, _, _) => (108, 36),
                (false, true, true, true, _, _, false, false) => (90, 0),
                (true, false, true, true, false, false, _, _) => (90, 36),
                (true, true, false, true, _, false, _, false) => (72, 18),
                (true, true, true, false, false, _, false, _) => (108, 18),
                (true, true, true, true, false, false, false, false) => (90, 18),
                (true, true, true, true, true, true, false, true) => (144, 36),
                (true, true, true, true, true, true, true, false) => (126, 36),
                (true, true, true, true, false, true, true, true) => (144, 54),
                (true, true, true, true, true, false, true, true) => (126, 54),
                (true, true, true, true, true, true, false, false) => (198, 0),
                (true, true, true, true, false, false, true, true) => (198, 18),
                (true, true, true, true, false, true, false, true) => (198, 36),
                (true, true, true, true, true, false, true, false) => (198, 54),
                (true, true, true, true, true, false, false, true) => (108, 54),
                (true, true, true, true, false, true, true, false) => (90, 54),
                (true, true, true, true, false, true, false, false) => (126, 18),
                (true, true, true, true, false, false, false, true) => (126, 0),
                (true, true, true, true, true, false, false, false) => (144, 18),
                (true, true, true, true, false, false, true, false) => (144, 0),
                (false, true, true, true, false, false, false, true) => (180, 0),
                (false, true, true, true, false, false, true, false) => (162, 0),
                (true, false, true, true, false, true, false, false) => (180, 18),
                (true, false, true, true, true, false, false, false) => (162, 18),
                (true, true, false, true, false, true, false, false) => (162, 36),
                (true, true, false, true, false, false, false, true) => (162, 54),
                (true, true, true, false, true, false, false, false) => (180, 36),
                (true, true, true, false, false, false, true, false) => (180, 54),
                _ => null
            };

            ApplyFrame(tile, frame);
        }

        private static bool ApplyFrame(Tile tile, (int X, int Y)? frame)
        {
            if (frame is { } selected)
            {
                tile.TileFrameX = (short)selected.X;
                tile.TileFrameY = (short)selected.Y;
                return true;
            }
            return false;
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

            const Similarity S = Similarity.Same;
            const Similarity M = Similarity.MergeLink;
            const Similarity N = Similarity.None;
            const int Up = 1;
            const int Left = 2;
            const int Right = 4;
            const int Down = 8;

            if (leftSim == S && upSim == S && downSim == S && rightSim == S)
            {
                // Diagonal merge links take priority, in bottom-right to top-left order.
                (int X, int Y) fullFrame = (topLeftSim, topRightSim, bottomLeftSim, bottomRightSim) switch
                {
                    (_, _, _, M) => (0, 90 + 36 * randomFrame),
                    (_, _, M, _) => (18, 90 + 36 * randomFrame),
                    (_, M, _, _) => (0, 108 + 36 * randomFrame),
                    (M, _, _, _) => (18, 108 + 36 * randomFrame),
                    (S, S, N, N) => (108 + 18 * randomFrame, 36),
                    (S, N, S, N) => (198, 18 * randomFrame),
                    _ => (18 + 18 * randomFrame, 18)
                };
                SetFrameAt(x, y, fullFrame.X, fullFrame.Y);
                return;
            }

            // Cardinal atlas table: left, up, down, right. Link flags are intentionally
            // separate from frame selection: several merge-shaped frames report no link.
            (int X, int Y, int Links) frame = (leftSim, upSim, downSim, rightSim) switch
            {
                (S, S, S, M) => (144, 126 + 18 * randomFrame, Right),
                (S, S, S, N) => (72, 18 * randomFrame, 0),
                (S, S, M, S) => (144 + 18 * randomFrame, 90, Down),
                (S, S, M, M) => (54, 108 + 36 * randomFrame, Right | Down),
                (S, S, M, N) => (90, 90 + 18 * randomFrame, Down),
                (S, S, N, S) => (18 + 18 * randomFrame, 36, 0),
                (S, S, N, M) => (54 + 18 * randomFrame, 216, Right),
                (S, S, N, N) => (18 + 36 * randomFrame, 72, 0),
                (S, M, S, S) => (144 + 18 * randomFrame, 108, Up),
                (S, M, S, M) => (54, 90 + 36 * randomFrame, Up | Right),
                (S, M, S, N) => (90, 144 + 18 * randomFrame, Up),
                (S, M, M, S) => (144 + 18 * randomFrame, 180, Up | Down),
                (S, M, M, M) => (216, 144 + 18 * randomFrame, Up | Right | Down),
                (S, M, M, N) => (216, 18 * randomFrame, 0),
                (S, M, N, S) => (234 + 18 * randomFrame, 18, Up),
                (S, M, N, M) => (216, 18 * randomFrame, 0),
                (S, M, N, N) => (216, 18 * randomFrame, 0),
                (S, N, S, S) => (18 + 18 * randomFrame, 0, 0),
                (S, N, S, M) => (54 + 18 * randomFrame, 198, Right),
                (S, N, S, N) => (18 + 36 * randomFrame, 54, 0),
                (S, N, M, S) => (234 + 18 * randomFrame, 0, Down),
                (S, N, M, M) => (216, 18 * randomFrame, 0),
                (S, N, M, N) => (216, 18 * randomFrame, 0),
                (S, N, N, S) => (108 + 18 * randomFrame, 72, 0),
                (S, N, N, M) => (54 + 18 * randomFrame, 252, Right),
                (S, N, N, N) => (216, 18 * randomFrame, 0),
                (M, S, S, S) => (162, 126 + 18 * randomFrame, Left),
                (M, S, S, M) => (180, 126 + 18 * randomFrame, Left | Right),
                (M, S, S, N) => (234 + 18 * randomFrame, 54, Left),
                (M, S, M, S) => (36, 108 + 36 * randomFrame, Left | Down),
                (M, S, M, M) => (198, 144 + 18 * randomFrame, Left | Right | Down),
                (M, S, M, N) => (108 + 18 * randomFrame, 54, 0),
                (M, S, N, S) => (18 * randomFrame, 216, Left),
                (M, S, N, M) => (108 + 18 * randomFrame, 54, 0),
                (M, S, N, N) => (108 + 18 * randomFrame, 54, 0),
                (M, M, S, S) => (36, 90 + 36 * randomFrame, Up | Left),
                (M, M, S, M) => (198, 90 + 18 * randomFrame, Up | Left | Right),
                (M, M, S, N) => (108 + 18 * randomFrame, 0, 0),
                (M, M, M, S) => (216, 90 + 18 * randomFrame, Up | Left | Down),
                (M, M, M, M) => (108 + 18 * randomFrame, 198, Up | Left | Right | Down),
                (M, M, M, N) => (162 + 18 * randomFrame, 54, 0),
                (M, M, N, S) => (162, 18 * randomFrame, 0),
                (M, M, N, M) => (162 + 18 * randomFrame, 54, 0),
                (M, M, N, N) => (162 + 18 * randomFrame, 54, 0),
                (M, N, S, S) => (18 * randomFrame, 198, Left),
                (M, N, S, M) => (108 + 18 * randomFrame, 0, 0),
                (M, N, S, N) => (108 + 18 * randomFrame, 0, 0),
                (M, N, M, S) => (162, 18 * randomFrame, 0),
                (M, N, M, M) => (162 + 18 * randomFrame, 54, 0),
                (M, N, M, N) => (162 + 18 * randomFrame, 54, 0),
                (M, N, N, S) => (18 * randomFrame, 252, Left),
                (M, N, N, M) => (162 + 18 * randomFrame, 198, Left | Right),
                (M, N, N, N) => (18 * randomFrame, 234, Left),
                (N, S, S, S) => (0, 18 * randomFrame, 0),
                (N, S, S, M) => (234 + 18 * randomFrame, 36, Right),
                (N, S, S, N) => (90, 18 * randomFrame, 0),
                (N, S, M, S) => (72, 90 + 18 * randomFrame, Down),
                (N, S, M, M) => (108 + 18 * randomFrame, 54, 0),
                (N, S, M, N) => (126, 90 + 18 * randomFrame, Down),
                (N, S, N, S) => (36 * randomFrame, 72, 0),
                (N, S, N, M) => (108 + 18 * randomFrame, 54, 0),
                (N, S, N, N) => (108 + 18 * randomFrame, 54, 0),
                (N, M, S, S) => (72, 144 + 18 * randomFrame, Up),
                (N, M, S, M) => (108 + 18 * randomFrame, 0, 0),
                (N, M, S, N) => (126, 144 + 18 * randomFrame, Up),
                (N, M, M, S) => (162, 18 * randomFrame, 0),
                (N, M, M, M) => (162 + 18 * randomFrame, 54, 0),
                (N, M, M, N) => (108, 216 + 18 * randomFrame, Up | Down),
                (N, M, N, S) => (162, 18 * randomFrame, 0),
                (N, M, N, M) => (162 + 18 * randomFrame, 54, 0),
                (N, M, N, N) => (108, 144 + 18 * randomFrame, Up),
                (N, N, S, S) => (36 * randomFrame, 54, 0),
                (N, N, S, M) => (108 + 18 * randomFrame, 0, 0),
                (N, N, S, N) => (108 + 18 * randomFrame, 0, 0),
                (N, N, M, S) => (162, 18 * randomFrame, 0),
                (N, N, M, M) => (162 + 18 * randomFrame, 54, 0),
                (N, N, M, N) => (108, 90 + 18 * randomFrame, Down),
                (N, N, N, S) => (162, 18 * randomFrame, 0),
                (N, N, N, M) => (54 + 18 * randomFrame, 234, Right),
                _ => (162 + 18 * randomFrame, 54, 0)
            };

            // Only write true flags after the original false initialization. In particular,
            // callers may alias out arguments, so assigning a whole tuple would be incorrect.
            if ((frame.Links & Up) != 0)
                mergedUp = true;
            if ((frame.Links & Left) != 0)
                mergedLeft = true;
            if ((frame.Links & Right) != 0)
                mergedRight = true;
            if ((frame.Links & Down) != 0)
                mergedDown = true;
            SetFrameAt(x, y, frame.X, frame.Y);
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

using System;
using System.Collections.Generic;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.Tiles.SunkenSea.Ambient;
using CalamityMod.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.World
{
    public class SunkenSea
    {
        private struct Hub
        {
            public Vector2 Position;

            public Hub(Vector2 position)
            {
                Position = position;
            }

            public Hub(float x, float y)
            {
                Position = new Vector2(x, y);
            }
        }

        private class Cluster : List<Hub>
        {
        }

        private class ClusterGroup : List<Cluster>
        {
            public int Width;

            public int Height;

            private void SearchForCluster(bool[,] hubMap, List<Point> pointCluster, int x, int y, int level = 2)
            {
                pointCluster.Add(new Point(x, y));
                hubMap[x, y] = false;
                level--;
                if (level == -1)
                {
                    return;
                }
                if (x > 0 && hubMap[x - 1, y])
                {
                    SearchForCluster(hubMap, pointCluster, x - 1, y, level);
                }
                if (x < hubMap.GetLength(0) - 1 && hubMap[x + 1, y])
                {
                    SearchForCluster(hubMap, pointCluster, x + 1, y, level);
                }
                if (y > 0 && hubMap[x, y - 1])
                {
                    SearchForCluster(hubMap, pointCluster, x, y - 1, level);
                }
                if (y < hubMap.GetLength(1) - 1 && hubMap[x, y + 1])
                {
                    SearchForCluster(hubMap, pointCluster, x, y + 1, level);
                }
            }

            private void AttemptClaim(int x, int y, int[,] clusterIndexMap, List<List<Point>> pointClusters, int index) //Attempts to create a cluster
            {
                int clusterPos = clusterIndexMap[x, y];
                if (clusterPos != -1 && clusterPos != index)
                {
                    int clusterHeight = (WorldGen.genRand.NextBool(2)) ? -1 : index;
                    foreach (Point current in pointClusters[clusterPos])
                    {
                        clusterIndexMap[current.X, current.Y] = clusterHeight;
                    }
                }
            }

            public void Generate(int width, int height) //Creates clusters for cluster group
            {
                Width = width;
                Height = height;
                Clear();
                bool[,] array = new bool[width, height];
                int clusterPos = (width >> 1) - 1;
                int clusterHeight = (height >> 1) - 1;
                int clusterSize = (clusterPos + 1) * (clusterPos + 1);
                Point point = new Point(clusterPos, clusterHeight);
                for (int i = point.Y - clusterHeight; i <= point.Y + clusterHeight; i++)
                {
                    float num4 = (float)clusterPos / (float)clusterHeight * (float)(i - point.Y);
                    int num5 = Math.Min(clusterPos, (int)Math.Sqrt((float)clusterSize - num4 * num4));
                    for (int j = point.X - num5; j <= point.X + num5; j++)
                    {
                        array[j, i] = WorldGen.genRand.NextBool(2);
                    }
                }
                List<List<Point>> list = new List<List<Point>>();
                for (int k = 0; k < array.GetLength(0); k++)
                {
                    for (int l = 0; l < array.GetLength(1); l++)
                    {
                        if (array[k, l] && WorldGen.genRand.NextBool(2))
                        {
                            List<Point> list2 = new List<Point>();
                            SearchForCluster(array, list2, k, l, 2);
                            if (list2.Count > 2)
                            {
                                list.Add(list2);
                            }
                        }
                    }
                }
                int[,] array2 = new int[array.GetLength(0), array.GetLength(1)];
                for (int m = 0; m < array2.GetLength(0); m++)
                {
                    for (int n = 0; n < array2.GetLength(1); n++)
                    {
                        array2[m, n] = -1;
                    }
                }
                for (int num6 = 0; num6 < list.Count; num6++)
                {
                    foreach (Point current in list[num6])
                    {
                        array2[current.X, current.Y] = num6;
                    }
                }
                for (int num7 = 0; num7 < list.Count; num7++)
                {
                    foreach (Point point2 in list[num7])
                    {
                        int x = point2.X;
                        int y = point2.Y;
                        if (array2[x, y] == -1)
                        {
                            break;
                        }
                        int index = array2[x, y];
                        if (x > 0)
                        {
                            AttemptClaim(x - 1, y, array2, list, index);
                        }
                        if (x < array2.GetLength(0) - 1)
                        {
                            AttemptClaim(x + 1, y, array2, list, index);
                        }
                        if (y > 0)
                        {
                            AttemptClaim(x, y - 1, array2, list, index);
                        }
                        if (y < array2.GetLength(1) - 1)
                        {
                            AttemptClaim(x, y + 1, array2, list, index);
                        }
                    }
                }
                using (List<List<Point>>.Enumerator enumerator2 = list.GetEnumerator())
                {
                    while (enumerator2.MoveNext())
                    {
                        enumerator2.Current.Clear();
                    }
                }
                for (int num8 = 0; num8 < array2.GetLength(0); num8++)
                {
                    for (int num9 = 0; num9 < array2.GetLength(1); num9++)
                    {
                        if (array2[num8, num9] != -1)
                        {
                            list[array2[num8, num9]].Add(new Point(num8, num9));
                        }
                    }
                }
                foreach (List<Point> current2 in list)
                {
                    if (current2.Count < 4)
                    {
                        current2.Clear();
                    }
                }
                foreach (List<Point> current3 in list)
                {
                    Cluster cluster = new Cluster();
                    if (current3.Count > 0)
                    {
                        foreach (Point current4 in current3)
                        {
                            cluster.Add(new Hub((float)current4.X + (WorldGen.genRand.NextFloat() - 0.5f) * 0.5f, (float)current4.Y + (WorldGen.genRand.NextFloat() - 0.5f) * 0.5f));
                        }
                        Add(cluster);
                    }
                }
            }
        }

        // Generates clumps / clusters / chunks of general tiles in the Sunken Sea area
        private static void PlaceClusters(ClusterGroup clusters, Point start, Vector2 terrainApplicationScaleVector)
        {
            int totalWidth = (int)(terrainApplicationScaleVector.X * clusters.Width);
            int totalHeight = (int)(terrainApplicationScaleVector.Y * clusters.Height);
            Vector2 totalScale = new Vector2(totalWidth, totalHeight);
            Vector2 individualScale = new Vector2(clusters.Width, clusters.Height);

            //I was a bit worried about editing anything in this loop too drastically, so i just changed all instances of placing navystone to eutrophic sand
            for (int i = -20; i < totalWidth + 20; i++)
            {
                for (int j = -20; j < totalHeight + 20; j++)
                {
                    float num3 = 0f;
                    int clusterAmt = -1;
                    float num5 = 0f;
                    int num6 = i + start.X;
                    int num7 = j + start.Y;
                    Vector2 vector = new Vector2((float)i, (float)j) / totalScale * individualScale;
                    float num8 = (new Vector2((float)i, (float)j) / totalScale * 2f - Vector2.One).Length();
                    for (int k = 0; k < clusters.Count; k++)
                    {
                        Cluster cluster = clusters[k];
                        if (Math.Abs(cluster[0].Position.X - vector.X) <= 10f && Math.Abs(cluster[0].Position.Y - vector.Y) <= 10f)
                        {
                            float num9 = 0f;
                            foreach (Hub current in cluster)
                            {
                                num9 += 1f / Vector2.DistanceSquared(current.Position, vector);
                            }
                            if (num9 > num3)
                            {
                                if (num3 > num5)
                                {
                                    num5 = num3;
                                }
                                num3 = num9;
                                clusterAmt = k;
                            }
                            else if (num9 > num5)
                            {
                                num5 = num9;
                            }
                        }
                    }
                    float num10 = num3 + num5;
                    Tile tile = Main.tile[num6, num7];
                    bool flag = num8 >= 0.8f;
                    if (num10 > 4.5f) //Adjust num10 for all cases if you want different tile frequencies; higher is less frequent, lower is more frequent
                    {
                        tile.ClearEverything();
                        tile.LiquidAmount = 192;
                        if (clusterAmt % 5 == 2)
                        {
                            ResetToEutrophicSand(tile, num6, num7, smoothBeforeLiquid: false, setSandWall: false);
                        }
                        Tile.SmoothSlope(num6, num7, true);
                    }
                    else if (num10 > 1.8f)
                    {
                        tile.WallType = (ushort)ModContent.WallType<NavystoneWall>();
                        tile.LiquidAmount = 192;
                        if (!flag || tile.HasTile)
                        {
                            ResetToEutrophicSand(tile, num6, num7, smoothBeforeLiquid: true, setSandWall: false);
                        }
                    }
                    else if (num10 > 0.7f || !flag)
                    {
                        tile.LiquidAmount = 192;
                        if (!flag || tile.HasTile)
                        {
                            ResetToEutrophicSand(tile, num6, num7, smoothBeforeLiquid: true, setSandWall: false);
                        }
                        tile.WallType = (ushort)ModContent.WallType<EutrophicSandWall>();
                    }
                    else if (num10 > 0.25f)
                    {
                        float num11 = (num10 - 0.25f) / 0.45f;
                        if (WorldGen.genRand.NextFloat() < num11)
                        {
                            if (tile.HasTile)
                            {
                                ResetToEutrophicSand(tile, num6, num7, smoothBeforeLiquid: true, setSandWall: true);
                            }
                            else
                            {
                                tile.WallType = (ushort)ModContent.WallType<NavystoneWall>();
                                tile.LiquidAmount = 192;
                            }
                        }
                    }
                }
            }

            //final cleanup loop
            for (int i = -20; i < totalWidth + 20; i++)
            {
                for (int j = -20; j < totalHeight + 20; j++)
                {
                    int x = i + start.X;
                    int y = j + start.Y;

                    Tile tile = Main.tile[x, y];
                    Tile tileUp = Main.tile[x, y - 1];
                    Tile tileDown = Main.tile[x, y + 1];
                    Tile tileDown2 = Main.tile[x, y + 2];
                    Tile tileLeft = Main.tile[x - 1, y];
                    Tile tileRight = Main.tile[x + 1, y];

                    //lava BEGONE
                    if (tile.WallType == ModContent.WallType<NavystoneWall>() || tile.WallType == ModContent.WallType<EutrophicSandWall>())
                    {
                        if (tile.LiquidType == LiquidID.Lava && tile.LiquidAmount > 0)
                        {
                            tile.LiquidType = LiquidID.Water;
                            tile.LiquidAmount = 255;
                        }

                        //also get rid of any annoying leftover obsidian
                        if (tile.TileType == TileID.Obsidian)
                        {
                            WorldGen.KillTile(x, y);
                        }
                    }

                    //place extra caves throughout the biome for variance
                    if (WorldGen.genRand.NextBool(1000) && tile != null && tile.HasTile && IsNavystoneOrSand(tile))
                    {
                        TileRunner runner = new TileRunner(new Vector2(x, y), new Vector2(0, 5), new Point16(-35, 35),
                        new Point16(-35, 35), 15f, WorldGen.genRand.Next(25, 50), 0, false, true);
                        runner.Start();
                    }

                    //kill any random floating tiles
                    if (IsNavystoneOrSand(tile))
                    {
                        //kill any individual floating tiles
                        if (!tileUp.HasTile && !tileDown.HasTile && !tileLeft.HasTile && !tileRight.HasTile)
                        {
                            WorldGen.KillTile(x, y);
                        }
                    }

                    //kill any random clumps of navystone without eutrophic sand around them
                    if (tile.TileType == ModContent.TileType<Navystone>() && tileUp.TileType != ModContent.TileType<EutrophicSand>() &&
                    tileDown.TileType != ModContent.TileType<EutrophicSand>() && tileLeft.TileType != ModContent.TileType<EutrophicSand>() &&
                    tileRight.TileType != ModContent.TileType<EutrophicSand>())
                    {
                        WorldGen.KillTile(x, y);
                    }
                }
            }
        }

        // Adds tile variation to generated tile clusters and generates open areas with sea prism ore called "Tits"
        // Generates sea prism crystals on prism ore and occasionally on navystone
        private static void AddGeodes(ClusterGroup clusters, Point start, Vector2 terrainApplicationScaleVector, float overallBiomeScale)
        {
            int totalClusterZoneWidth = (int)(terrainApplicationScaleVector.X * clusters.Width);
            int totalClusterZoneHeight = (int)(terrainApplicationScaleVector.Y * clusters.Height);
            bool genCentalHole = true;
            Rectangle rectangle = default;

            // Radius of the generated hole
            int radius = (int)(WorldGen.genRand.Next(24, 28) * overallBiomeScale);
            int diameter = radius * 2;

            // Where to place the giant hole? "Center of the Sunken Sea", defined as follows:
            // Dead center horizontally, and 1/3 of the way down the area
            Point point = new Point(start.X + totalClusterZoneWidth / 2, start.Y + (int)(totalClusterZoneHeight * 0.33f));

            ShapeData holeShape = new ShapeData();
            float outerRadiusPercentage = WorldGen.genRand.Next(40, 56) * 0.01f; //Small radius for ore patch to fit inside holes

            // Y coordinate of the "bottom" of the Sunken Sea, 70% of the way down.
            int sunkenSeaBottom = start.Y + (int)(totalClusterZoneHeight * 0.7f);
            int smallHoles = 0;

            // Scale amount of holes with world size
            // 4 on Small, 6 on Normal (rounds down), 8 on Large
            int totalHoleCount = (int)(4f * overallBiomeScale);

            for (int i = -20; i < totalClusterZoneWidth + 20; i++)
            {
                for (int j = -20; j < totalClusterZoneHeight + 20; j++)
                {
                    if (genCentalHole)
                    {
                        // Set the rectangle for the central hole
                        genCentalHole = false;
                        rectangle = new Rectangle(start.X + totalClusterZoneWidth / 2 - radius, start.Y + (int)(totalClusterZoneHeight * 0.33f) - radius, diameter, diameter);

                        GenerateHoleOpening(point, radius, holeShape, 0.6, 4);
                        GenerateHoleShell(point, radius, outerRadiusPercentage, holeShape, radius, 3);
                    }

                    // Set the point for non-central holes
                    int smallHoleX = WorldGen.genRand.Next(start.X + 30, start.X + totalClusterZoneWidth - 30);
                    int smallHoleY = WorldGen.genRand.Next(start.Y + 20, sunkenSeaBottom);
                    point = new Point(smallHoleX, smallHoleY);

                    if (smallHoles < totalHoleCount && WorldGen.genRand.NextBool(3) && !rectangle.Contains(point))
                    {
                        smallHoles++;
                        int radiusSmall = (int)(((float)WorldGen.genRand.Next(8, 11)) * overallBiomeScale);
                        GenerateHoleOpening(point, radiusSmall, holeShape, 0.45, 3);

                        outerRadiusPercentage = (float)((double)WorldGen.genRand.Next(65, 81) * 0.01);
                        GenerateHoleShell(point, radiusSmall, outerRadiusPercentage, holeShape, radius, 3);
                    }

                    int num3 = i + start.X;
                    int num4 = j + start.Y;
                    Tile tile = Main.tile[num3, num4];
                    Tile testTile = Main.tile[num3, num4 + 1];
                    Tile testTile2 = Main.tile[num3, num4 + 2];
                    if (tile.TileType == ModContent.TileType<EutrophicSand>() && (!WorldGen.SolidTile(testTile) || !WorldGen.SolidTile(testTile2))) //Tile variation
                    {
                        tile.TileType = (ushort)ModContent.TileType<Navystone>();
                    }
                }
            }
        }

        private static bool IsNavystoneOrSand(Tile tile) => tile.TileType == ModContent.TileType<Navystone>() || tile.TileType == ModContent.TileType<EutrophicSand>();

        private static void ResetToEutrophicSand(Tile tile, int x, int y, bool smoothBeforeLiquid, bool setSandWall)
        {
            tile.ResetToType((ushort)ModContent.TileType<EutrophicSand>());
            tile.Get<TileWallWireStateData>().HasTile = true;
            if (smoothBeforeLiquid)
                Tile.SmoothSlope(x, y, true);
            if (setSandWall)
                tile.WallType = (ushort)ModContent.WallType<EutrophicSandWall>();
            tile.LiquidAmount = 0;
        }

        private static void PlaceDecoration<T>(int x, int y, int chance, bool placeAsObject = false, int styleCount = 0) where T : ModTile
        {
            if (!WorldGen.genRand.NextBool(chance))
            {
                return;
            }

            int tileType = ModContent.TileType<T>();
            int style = styleCount > 0 ? WorldGen.genRand.Next(styleCount) : 0;
            if (placeAsObject)
                WorldGen.PlaceObject(x, y, tileType, style: style);
            else
                WorldGen.PlaceTile(x, y, (ushort)tileType, true, false, -1, style);
        }

        private static bool HasThreeOpenTiles(int x, int y, int offsetX, int offsetY)
        {
            for (int distance = 1; distance <= 3; distance++)
            {
                if (Main.tile[x + offsetX * distance, y + offsetY * distance].HasTile)
                {
                    return false;
                }
            }

            return true;
        }

        private static void PlaceCrystalOnExposedSide(Tile source, bool exposedFirst, bool exposedSecond, int x, int y, bool horizontal, int firstFrame, int secondFrame)
        {
            if (!(exposedFirst ^ exposedSecond) || source.Slope != 0 || source.IsHalfBlock)
            {
                return;
            }

            int firstX = x + (horizontal ? -1 : 0);
            int firstY = y + (horizontal ? 0 : -1);
            int secondX = x + (horizontal ? 1 : 0);
            int secondY = y + (horizontal ? 0 : 1);
            Tile crystal = Main.tile[x + (horizontal ? (exposedFirst ? -1 : 1) : 0), y + (horizontal ? 0 : (exposedFirst ? -1 : 1))];
            ushort crystalType = (ushort)ModContent.TileType<SeaPrismCrystals>();
            crystal.TileType = crystalType;

            Tile firstNeighbor = Main.tile[firstX, firstY];
            if (firstNeighbor.TileType == crystalType)
            {
                firstNeighbor.TileFrameY = (short)firstFrame;
            }
            else
            {
                Tile secondNeighbor = Main.tile[secondX, secondY];
                if (secondNeighbor.TileType == crystalType)
                {
                    secondNeighbor.TileFrameY = (short)secondFrame;
                }
            }

            crystal.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
            crystal.Get<TileWallWireStateData>().HasTile = true;
        }

        private static void GenerateHoleOpening(Point point, int radius, ShapeData holeShape, double blotchChance, int smoothingChance)
        {
            WorldUtils.Gen(point, new Shapes.Circle(radius), Actions.Chain(new GenAction[]
            {
                new Modifiers.Blotches(2, blotchChance).Output(holeShape),
                new Actions.ClearTile(true),
                new Actions.SetLiquid(0, 192)
            }));

            SmoothHoleOutline(point, holeShape, radius, smoothingChance);
        }

        private static void GenerateHoleShell(Point point, int radius, float outerRadiusPercentage, ShapeData holeShape, int innerOutlineRadius, int smoothingChance)
        {
            WorldUtils.Gen(point, new Shapes.Circle((int)((float)radius * outerRadiusPercentage)), Actions.Chain(new GenAction[]
            {
                new Modifiers.Blotches(2, 0.3).Output(holeShape),
                new Actions.SetTile((ushort)ModContent.TileType<Navystone>(), true)
            }));

            SmoothHoleOutline(point, holeShape, radius, smoothingChance);

            WorldUtils.Gen(point, new Shapes.Circle((int)((float)radius * (outerRadiusPercentage * 0.6f))), Actions.Chain(new GenAction[]
            {
                new Modifiers.Blotches(2, 0.3),
                new Actions.SetTile((ushort)ModContent.TileType<SeaPrism>(), true)
            }));

            WorldUtils.Gen(point, new Shapes.Circle((int)((float)radius * (outerRadiusPercentage * 0.3f))), Actions.Chain(new GenAction[]
            {
                new Modifiers.Blotches(2, 0.3).Output(holeShape),
                new Actions.ClearTile(true),
                new Actions.SetLiquid(0, 255)
            }));

            SmoothHoleOutline(point, holeShape, innerOutlineRadius, 2);
        }

        private static void SmoothHoleOutline(Point point, ShapeData holeShape, int radius, int chance)
        {
            WorldUtils.Gen(point, new ModShapes.OuterOutline(holeShape, true, true), Actions.Chain(new GenAction[]
            {
                new CustomActions.DistanceFromOrigin(true, radius * 16f - 48),
                new Modifiers.Conditions(new CustomConditions.RandomChance(chance)),
                new Actions.Smooth(),
                new Actions.SetFrames(true)
            }));
        }

        private static void AddTileVariance(ClusterGroup clusters, Point start, Vector2 terrainApplicationScaleVector, float overallBiomeScale)
        {
            int totalClusterZoneWidth = (int)(terrainApplicationScaleVector.X * clusters.Width);
            int totalClusterZoneHeight = (int)(terrainApplicationScaleVector.Y * clusters.Height);
            for (int k = -20; k < totalClusterZoneWidth + 20; k++)
            {
                for (int l = -20; l < totalClusterZoneHeight + 20; l++)
                {
                    int num5 = k + start.X;
                    int num6 = l + start.Y;
                    Tile tile = Main.tile[num5, num6];
                    if (tile.HasTile && (tile.TileType == ModContent.TileType<SeaPrism>() || IsNavystoneOrSand(tile)))
                    {
                        bool flag = HasThreeOpenTiles(num5, num6, 0, -1);
                        bool flag2 = HasThreeOpenTiles(num5, num6, 0, 1);
                        bool flag3 = HasThreeOpenTiles(num5, num6, -1, 0);
                        bool flag4 = HasThreeOpenTiles(num5, num6, 1, 0);
                        if (tile.TileType == ModContent.TileType<SeaPrism>() || (IsNavystoneOrSand(tile) && WorldGen.genRand.NextBool(8)))
                        {
                            PlaceCrystalOnExposedSide(tile, flag3, flag4, num5, num6, true, 2 * 18, 3 * 18);
                            PlaceCrystalOnExposedSide(tile, flag, flag2, num5, num6, false, 0, 1 * 18);
                        }
                    }
                    if (tile.TileType == ModContent.TileType<Navystone>() || tile.TileType == ModContent.TileType<EutrophicSand>())
                    {
                        PlaceDecoration<BrainCoral>(num5, num6 - 1, 10);
                        PlaceDecoration<SmallBrainCoral>(num5, num6 - 1, 20);
                        PlaceDecoration<FanCoral>(num5, num6 - 1, 8);
                        PlaceDecoration<TubeCoral>(num5, num6 - 1, 30);
                        PlaceDecoration<SmallTubeCoral>(num5, num6 - 1, 30);
                        PlaceDecoration<SeaAnemone>(num5, num6 - 1, 20);

                        //colorful corals
                        PlaceDecoration<MediumCoral>(num5, num6 - 1, 20);
                        PlaceDecoration<MediumCoral2>(num5, num6 - 1, 20);
                        PlaceDecoration<SmallWideCoral>(num5, num6 - 1, 20);
                        PlaceDecoration<CoralPileLarge>(num5, num6 - 1, 20);

                        //stalactites
                        PlaceDecoration<SunkenStalactites>(num5, num6 + 2, 15, placeAsObject: true, styleCount: 3);
                        PlaceDecoration<SmallSunkenStalactites>(num5, num6 + 1, 15, placeAsObject: false, styleCount: 3);

                        //stalagmites
                        PlaceDecoration<SunkenStalagmites>(num5, num6 - 2, 10, placeAsObject: true, styleCount: 3);
                        PlaceDecoration<SmallSunkenStalagmites>(num5, num6 - 1, 10, placeAsObject: false, styleCount: 3);
                    }
                    if (!tile.HasTile && (tile.TileType == ModContent.TileType<Navystone>() || tile.WallType == ModContent.WallType<EutrophicSandWall>()))
                    {
                        PlaceDecoration<TableCoral>(num5, num6 + 1, 10);
                    }
                }
            }
        }

        //added this to 100% make sure the sunken sea places where it is supposed to be
        static bool foundValidPosition = false;

        public static bool Place(Point origin)
        {
            for (int y = origin.Y; y >= (int)Main.worldSurface; y--)
            {
                //check for the desert biomes walls
                if (Main.tile[origin.X, y].WallType == WallID.Sandstone || Main.tile[origin.X, y].WallType == WallID.HardenedSand)
                {
                    origin.Y = y + 50; //offset so it doesnt generate weird
                    foundValidPosition = true;
                    break;
                }
            }

            while (foundValidPosition)
            {
                // 1 on Small, 1.52 on Medium, 2 on Large
                float scale = Main.maxTilesX / 4200f;
                // Clamp scale to prevent problems on extra large worlds
                scale = MathHelper.Clamp(scale, 1f, 2f);

                // 80 on Small, 121.6 on Medium, 160 on Large
                int sunkenSeaAreaX = (int)(80f * scale);
                // 84-102 on Small, 127.68-155.04 on Medium, 168-204 on Large
                float baseVerticalSize = 60f * scale;
                float verticalScaleFactor = 1.4f + WorldGen.genRand.NextFloat(0.3f);
                int sunkenSeaAreaY = (int)(verticalScaleFactor * baseVerticalSize);

                // What the fuck is this and why is it used everywhere
                // As far as I can tell, this just scales up the entire Sunken Sea to be 4x wider and 2x taller than what is listed above
                Vector2 arbitrary42GodVector = new Vector2(4f, 2f);

                // 04NOV2024: Ozzatron: Remnants mutilates the Underground Desert beyond recognition.
                // The above code assumes the size of the underground desert with arbitrary hardcoded values instead of checking it.
                // To ensure the Sunken Sea is centered on an Underground Desert of any size, some adjustments are made.
                // This would have been easy, but the arbitrary 4-2 God vector makes the math unnecessarily complicated.
                int sunkenSeaRealWidth = (int)(sunkenSeaAreaX * arbitrary42GodVector.X);
                origin.X = GenVars.UndergroundDesertLocation.Center.X - sunkenSeaRealWidth / 2;

                // Place the majority of the terrain as clusters
                ClusterGroup clusterGroup = new ClusterGroup();
                clusterGroup.Generate(sunkenSeaAreaX, sunkenSeaAreaY);
                PlaceClusters(clusterGroup, origin, arbitrary42GodVector);

                // Place Geodes
                AddGeodes(clusterGroup, origin, arbitrary42GodVector, scale);

                // Re-frame everything in some arbitrary radius
                int totalWidth = (int)(arbitrary42GodVector.X * clusterGroup.Width);
                int totalHeight = (int)(arbitrary42GodVector.Y * clusterGroup.Height);
                int frameExcessRadius = 40;
                for (int i = -frameExcessRadius; i < totalWidth + frameExcessRadius; i++)
                {
                    for (int j = -frameExcessRadius; j < totalHeight + frameExcessRadius; j++)
                    {
                        if (i + origin.X > 0 && i + origin.X < Main.maxTilesX - 1 && j + origin.Y > 0 && j + origin.Y < Main.maxTilesY - 1)
                        {
                            WorldGen.SquareWallFrame(i + origin.X, j + origin.Y, true);
                            WorldUtils.TileFrame(i + origin.X, j + origin.Y, true);
                            Tile.SmoothSlope(i + origin.X, j + origin.Y, true);
                        }
                    }
                }

                // Add Tile Variance (sand, crystals, etc.)
                AddTileVariance(clusterGroup, origin, arbitrary42GodVector, scale);

                return true;
            }

            return false;
        }
    }
}

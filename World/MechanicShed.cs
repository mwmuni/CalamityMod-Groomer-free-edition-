using System;
using System.Collections.Generic;
using CalamityMod.Schematics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;
using static CalamityMod.Schematics.SchematicManager;

namespace CalamityMod.World
{
    public class MechanicShed
    {
        public static void PlaceMechanicShed(StructureMap structures)
        {
            string mapKey = MechanicShedKey;
            var schematic = TileMaps[mapKey];

            int leftLimit = GenVars.snowOriginLeft + 100;
            int rightLimit = GenVars.snowOriginRight - 100;
            int placementPositionX = WorldGen.genRand.Next(leftLimit, rightLimit);
            int placementPositionY = (int)Main.worldSurface - 50;

            int distanceToCheckForSnowTilesX = 30;
            int distanceToCheckForSnowTilesY = 5;
            int snowTilesRequired = 100;
            int emptyTilesRequired = 100;

            bool foundValidGround = false;
            int attempts = 0;
            int maxAttempts = 100000;
            while (!foundValidGround && attempts <= maxAttempts)
            {
                attempts++;

                // Check if there are enough snow tiles on the bottom of the shed's gen location
                // 150 tiles checked in total
                int snowTileCount = 0;
                bool enoughSnowTilesOnBottom = false;
                for (int shedTileCheckIndexX = placementPositionX - 15; shedTileCheckIndexX < placementPositionX - 15 + distanceToCheckForSnowTilesX; shedTileCheckIndexX++)
                {
                    if (enoughSnowTilesOnBottom)
                        break;

                    for (int shedTileCheckIndexY = placementPositionY - 5; shedTileCheckIndexY < placementPositionY - 5 + distanceToCheckForSnowTilesY; shedTileCheckIndexY++)
                    {
                        if (Main.tile[shedTileCheckIndexX, shedTileCheckIndexY] != null)
                        {
                            if (Main.tile[shedTileCheckIndexX, shedTileCheckIndexY].HasTile)
                            {
                                if (Main.tile[shedTileCheckIndexX, shedTileCheckIndexY].TileType == TileID.SnowBlock)
                                {
                                    snowTileCount++;
                                    if (snowTileCount >= snowTilesRequired)
                                    {
                                        enoughSnowTilesOnBottom = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                // Check if there are enough empty tiles on the top of the shed's gen location
                // 150 tiles checked in total
                int emptyTileCount = 0;
                bool enoughEmptyTilesOnTop = false;
                for (int shedTileCheckIndexX = placementPositionX - 15; shedTileCheckIndexX < placementPositionX - 15 + distanceToCheckForSnowTilesX; shedTileCheckIndexX++)
                {
                    if (enoughEmptyTilesOnTop)
                        break;

                    for (int shedTileCheckIndexY = placementPositionY - 20; shedTileCheckIndexY < placementPositionY - 20 + distanceToCheckForSnowTilesY; shedTileCheckIndexY++)
                    {
                        if ((Main.tile[shedTileCheckIndexX, shedTileCheckIndexY] == null ||
                            !Main.tile[shedTileCheckIndexX, shedTileCheckIndexY].HasTile) &&
                            Main.tile[shedTileCheckIndexX, shedTileCheckIndexY].WallType == WallID.None)
                        {
                            emptyTileCount++;
                            if (emptyTileCount >= emptyTilesRequired)
                            {
                                enoughEmptyTilesOnTop = true;
                                break;
                            }
                        }
                    }
                }

                if (enoughSnowTilesOnBottom && enoughEmptyTilesOnTop)
                {
                    break;
                }
                else
                {
                    placementPositionX += 5;
                    if (placementPositionX > rightLimit)
                        placementPositionX = leftLimit;

                    if (!enoughEmptyTilesOnTop)
                        placementPositionY -= 5;
                    else if (!enoughSnowTilesOnBottom)
                        placementPositionY += 5;
                }
            }

            Point placementPoint = new Point(placementPositionX, placementPositionY);
            SchematicAnchor anchorType = SchematicAnchor.BottomCenter;

            bool place = true;
            PlaceSchematic(mapKey, placementPoint, anchorType, ref place, new Action<Chest, int, bool>(FillMechanicChest));

            Rectangle protectionArea = CalamityUtils.GetSchematicProtectionArea(schematic, placementPoint, anchorType);
            CalamityUtils.AddProtectedStructure(protectionArea, 30);
        }

        public static void FillMechanicChest(Chest chest, int Type, bool place)
        {
            int gizmoGoobabGadgets = Utils.SelectRandom(WorldGen.genRand, ItemID.BrickLayer, ItemID.ExtendoGrip, ItemID.PaintSprayer, ItemID.PortableCementMixer);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(ItemID.Toolbox, 1),
                new ChestItem(ItemID.ActuationAccessory, 1),
                new ChestItem(gizmoGoobabGadgets, 1),
                new ChestItem(ItemID.BuilderPotion, WorldGen.genRand.Next(1, 3)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(1, 3)),
            };

            MiscWorldgenRoutines.FillChestContents(chest, contents, prefixItems: true);
        }
    }
}

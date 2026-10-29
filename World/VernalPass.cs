using System;
using System.Collections.Generic;
using CalamityMod.Schematics;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static CalamityMod.Schematics.SchematicManager;

namespace CalamityMod.World
{
    public class VernalPass
    {
        public static void PlaceVernalPass(StructureMap structures)
        {
            string mapKey = VernalKey;
            var schematic = TileMaps[mapKey];

            int placementPositionX = WorldGen.genRand.Next(GenVars.tLeft, GenVars.tRight);

            //initialPlacementPositionY is the exact same variable as placementPositionY, but it is used to store the original starting position of the vernal pass
            //this is to prevent any cases of infinite looping in the loop below when the vernal pass y-position is increased/decreased to move away from the temple and find a valid position
            int initialPlacementPositionY = GenVars.tTop < Main.maxTilesY / 2  ? GenVars.tBottom + 150 : GenVars.tTop - 150;
            int placementPositionY = GenVars.tTop < Main.maxTilesY / 2  ? GenVars.tBottom + 150 : GenVars.tTop - 150;

            //attempt to find a valid position for the biome to place in
            bool foundValidPosition = false;
            int attempts = 0;

            //continously place up/down until it is far enough from the temple to not destroy it
            while (!foundValidPosition && attempts++ < 100000)
            {
                while (!NoTempleNearby(placementPositionX, placementPositionY))
                {
                    //increase the actual y-position the vernal pass will place at based on the starting location, if the jungle temple is too close to the initial starting location
                    placementPositionY += (initialPlacementPositionY < (Main.maxTilesX / 2) ? -10 : 10);
                }
                if (NoTempleNearby(placementPositionX, placementPositionY))
                {
                    foundValidPosition = true;
                }
            }

            Point placementPoint = new Point(placementPositionX, placementPositionY);

            Vector2 schematicSize = new Vector2(schematic.GetLength(0), schematic.GetLength(1));
            SchematicAnchor anchorType = SchematicAnchor.Center;

            bool firstItem = false;

            PlaceSchematic(mapKey, placementPoint, anchorType, ref firstItem, new Action<Chest, int, bool>(FillVernalPassChests));

            // Add the Vernal Pass as a protected structure.
            Rectangle protectionArea = CalamityUtils.GetSchematicProtectionArea(schematic, placementPoint, anchorType);
            CalamityUtils.AddProtectedStructure(protectionArea, 30);
        }

        public static void FillVernalPassChests(Chest chest, int Type, bool firstItem)
        {
            int mainItem = Utils.SelectRandom(WorldGen.genRand, ItemID.StaffofRegrowth, ItemID.AnkletoftheWind, ItemID.FeralClaws, ModContent.ItemType<Items.Weapons.Summon.BelladonnaSpiritStaff>());

            int bars = Utils.SelectRandom(WorldGen.genRand, ItemID.GoldBar, ItemID.PlatinumBar);
            int potionType = Utils.SelectRandom(WorldGen.genRand, ItemID.ThornsPotion, ItemID.BattlePotion, ItemID.ShinePotion, ItemID.HunterPotion);
            List<ChestItem> contents = new List<ChestItem>()
            {
                new ChestItem(bars, WorldGen.genRand.Next(4, 7)),
                new ChestItem(ItemID.JungleSpores, WorldGen.genRand.Next(4, 8)),
                new ChestItem(ItemID.Stinger, WorldGen.genRand.Next(2, 5)),
                new ChestItem(ItemID.JungleTorch, WorldGen.genRand.Next(2, 5)),
                new ChestItem(potionType, WorldGen.genRand.Next(1, 4)),
                new ChestItem(ItemID.GoldCoin, WorldGen.genRand.Next(1, 3)),
            };

            if (!firstItem)
            {
                contents.Insert(0, new ChestItem(ModContent.ItemType<Items.Tools.FellerofEvergreens>(), 1));
            }
            else
            {
                contents.RemoveAt(0);
                contents.Insert(0, new ChestItem(mainItem, 1));
                contents.Insert(1, new ChestItem(bars, WorldGen.genRand.Next(4, 7)));
            }

            MiscWorldgenRoutines.FillChestContents(chest, contents, prefixItems: true);
        }

        //determine if theres no snow blocks nearby so the biome doesnt place in the snow biome
        public static bool NoTempleNearby(int X, int Y)
        {
            for (int i = X - 140; i < X + 140; i++)
            {
                for (int j = Y - 140; j < Y + 140; j++)
                {
                    if (Main.tile[i, j].HasTile && Main.tile[i, j].TileType == TileID.LihzahrdBrick)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}

using System.Collections.Generic;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class CalamityGlobalItemLoot : GlobalItem
    {
        public override bool InstancePerEntity => false;

        #region Modify Item Loot Main Hook
        public override void ModifyItemLoot(Item item, ItemLoot loot)
        {
            Fraction fifteenPercent = new Fraction(15, 100);

            switch (item.type)
            {
                #region Boss Treasure Bags
                case ItemID.KingSlimeBossBag:
                    loot.DefineConditionalDropSet(DropHelper.NotRemix).Add(ItemID.Katana, DropHelper.BagWeaponDropRateInt); // 33% Katana
                    loot.DefineConditionalDropSet(DropHelper.Remix).Add(ItemID.Keybrand, DropHelper.BagWeaponDropRateInt); // 33% Keybrand in Remix

                    loot.Add(ModContent.ItemType<CrownJewel>(), DropHelper.BagWeaponDropRateFraction); // 33% Crown Jewel
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.EyeOfCthulhuBossBag:
                    loot.Add(ModContent.ItemType<DeathstareRod>(), DropHelper.BagWeaponDropRateFraction); // 33% Deathstare Rod
                    loot.Add(ModContent.ItemType<TeardropCleaver>(), DropHelper.BagWeaponDropRateFraction); // 33% Teardrop Cleaver
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                // On Rev+, Eater of Worlds segments don't drop partial loot. As such, the bag needs to drop all materials.
                // This can theoretically be exploited by killing the boss on Expert, then turning on Rev to open the bags.
                // We don't care.
                case ItemID.EaterOfWorldsBossBag:
                    var eowRevLCR = loot.DefineConditionalDropSet(DropHelper.If(() => CalamityWorld.revenge));
                    eowRevLCR.Add(ItemID.DemoniteOre, 1, 70, 90); // 100% 70-90 Demonite Ore
                    eowRevLCR.Add(ItemID.ShadowScale, 1, 20, 30); // 100% 20-30 Shadow Scale
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;


                // On Rev+, Brain of Cthulhu's Creepers don't drop partial loot. As such, the bag needs to drop all materials.
                // This can theoretically be exploited by killing the boss on Expert, then turning on Rev to open the bags.
                // We don't care.
                case ItemID.BrainOfCthulhuBossBag:
                    var bocRevLCR = loot.DefineConditionalDropSet(DropHelper.If(() => CalamityWorld.revenge));
                    bocRevLCR.Add(ItemID.CrimtaneOre, 1, 70, 90); // 100% 70-90 Crimtane Ore
                    bocRevLCR.Add(ItemID.TissueSample, 1, 20, 30); // 100% 20-30 Tissue Sample
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.DeerclopsBossBag:
                    loot.Remove(FindRuleWithItem<OneFromOptionsNotScaledWithLuckDropRule>(loot.Get(false), ItemID.LucyTheAxe));
                    int[] deerclopsWeapons = new int[]
                    {
                        ItemID.LucyTheAxe,
                        ItemID.PewMaticHorn,
                        ItemID.WeatherPain,
                        ItemID.HoundiusShootius,
                        ModContent.ItemType<WalkingCane>()
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, deerclopsWeapons));
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.QueenBeeBossBag:
                    loot.Remove(FindRuleWithItem<OneFromOptionsNotScaledWithLuckDropRule>(loot.Get(false), ItemID.BeeKeeper));
                    int[] queenBeeWeapons = new int[]
                    {
                        ItemID.BeeKeeper,
                        ItemID.BeesKnees,
                        ItemID.BeeGun,
                        ModContent.ItemType<HardenedHoneycomb>(),
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, queenBeeWeapons));
                    loot.Add(ModContent.ItemType<TheBee>(), DropHelper.BagWeaponDropRateFraction); // 33% The Bee
                    loot.Add(ItemID.Stinger, 1, 8, 12); // 100% 8-12 Stinger
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.WallOfFleshBossBag:
                    loot.Remove(FindRuleWithItem<OneFromOptionsNotScaledWithLuckDropRule>(loot.Get(false), ItemID.BreakerBlade));
                    loot.Remove(FindRuleWithItem<OneFromOptionsNotScaledWithLuckDropRule>(loot.Get(false), ItemID.WarriorEmblem));
                    int[] wofWeapons = new int[]
                    {
                        ItemID.BreakerBlade,
                        ModContent.ItemType<Carnage>(),
                        ItemID.ClockworkAssaultRifle,
                        ModContent.ItemType<Meowthrower>(),
                        ItemID.LaserRifle,
                        ModContent.ItemType<BlackHawkRemote>(),
                        ItemID.FireWhip, // Firecracker
                        ModContent.ItemType<BlastBarrel>()
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, wofWeapons));

                    int[] emblems = new int[]
                    {
                        ItemID.WarriorEmblem,
                        ItemID.RangerEmblem,
                        ItemID.SorcererEmblem,
                        ItemID.SummonerEmblem,
                        ModContent.ItemType<RogueEmblem>(),
                    };
                    loot.Add(DropHelper.CalamityStyle(new Fraction(1, 4), emblems)); // Emblems remain 25%
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.QueenSlimeBossBag:
                    loot.Add(ItemID.SoulofLight, 1, 15, 20); // 100% 15-20 Soul of Light
                    loot.Add(ItemID.PinkGel, 1, 15, 20); // 100% 15-20 Pink Gel
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.DestroyerBossBag:
                case ItemID.TwinsBossBag:
                case ItemID.SkeletronPrimeBossBag:
                    loot.Remove(FindRuleWithItem<CommonDrop>(loot.Get(false), ItemID.HallowedBar));
                    loot.AddIf(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 20, 35);
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.PlanteraBossBag:
                    // Specifically look up the Grenade Launcher because it has unique rules for its ammo.
                    loot.Remove(FindRuleWithItem<OneFromRulesRule>(loot.Get(false), ItemID.GrenadeLauncher));
                    int[] planteraWeapons = new int[]
                    {
                        ItemID.FlowerPow,
                        ItemID.Seedler,
                        ItemID.GrenadeLauncher,
                        ItemID.VenusMagnum,
                        ItemID.LeafBlower,
                        ItemID.NettleBurst,
                        ItemID.WaspGun
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, planteraWeapons));
                    loot.Add(ModContent.ItemType<BlossomFlux>(), 10); // 10% Blossom Flux
                    loot.Add(ModContent.ItemType<BloomStone>(), DropHelper.BagWeaponDropRateFraction);
                    loot.Add(ModContent.ItemType<LivingShard>(), 1, 40, 50);
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.GolemBossBag:
                    // Specifically look up the Stynger because it has unique rules for its ammo.
                    loot.Remove(FindRuleWithItem<OneFromRulesRule>(loot.Get(false), ItemID.Stynger));
                    int[] golemItems = new int[]
                    {
                        ItemID.GolemFist,
                        ItemID.PossessedHatchet,
                        ItemID.Stynger,
                        ItemID.HeatRay,
                        ItemID.StaffofEarth,
                        ItemID.EyeoftheGolem,
                        ItemID.SunStone
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, golemItems));
                    loot.Add(ModContent.ItemType<AegisBlade>(), 10); // 10% Aegis Blade
                    loot.Add(ModContent.ItemType<EssenceofSunlight>(), 1, 10, 12); // 100% 10-12 Essence of Sunlight
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.BossBagBetsy:
                    loot.Remove(FindRuleWithItem<OneFromOptionsNotScaledWithLuckDropRule>(loot.Get(false), ItemID.DD2SquireBetsySword));
                    int[] betsyWeapons = new int[]
                    {
                        ItemID.DD2SquireBetsySword, // Flying Dragon
                        ItemID.MonkStaffT3, // Sky Dragon's Fury
                        ItemID.DD2BetsyBow, // Aerial Bane
                        ItemID.ApprenticeStaffT3, // Betsy's Wrath
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, betsyWeapons));
                    break;

                case ItemID.FishronBossBag:
                    RemoveDukeRules(loot); // Separately remove Duke's weapons and wings, because they're both included below
                    int[] dukeItems = new int[]
                    {
                        ItemID.Flairon,
                        ItemID.Tsunami,
                        ItemID.BubbleGun,
                        ItemID.RazorbladeTyphoon,
                        ItemID.TempestStaff,
                        ModContent.ItemType<DukesDecapitator>(),
                        ItemID.FishronWings, // Duke's wings have a pathetically low drop rate.
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, dukeItems));
                    loot.Add(ModContent.ItemType<BrinyBaron>(), 10); // 10% Briny Baron
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.FairyQueenBossBag:
                    RemoveEmpressRules(loot); // Separately remove EoL's weapons and wings, because they're both included below
                    int[] empressItems = new int[]
                    {
                        ItemID.PiercingStarlight, // Starlight
                        ItemID.FairyQueenRangedItem, // Eventide
                        ItemID.FairyQueenMagicItem, // Nightglow
                        ItemID.SparkleGuitar, // Stellar Tune
                        ItemID.RainbowWhip, // Kaleidoscope
                        ItemID.RainbowWings, // Empress Wings have a pathetically low drop rate.
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, empressItems));
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.MoonLordBossBag:
                    loot.Remove(FindRuleWithItem<OneFromOptionsNotScaledWithLuckDropRule>(loot.Get(false), ItemID.Terrarian));
                    int[] moonLordWeapons = new int[]
                    {
                        ItemID.Meowmere,
                        ItemID.StarWrath,
                        ItemID.Terrarian,
                        ItemID.Celeb2, // Celebration Mk II
                        ItemID.SDMG,
                        ItemID.LastPrism,
                        ItemID.LunarFlareBook,
                        ItemID.MoonlordTurretStaff, // Lunar Portal Staff
                        ItemID.RainbowCrystalStaff,
                        ModContent.ItemType<UtensilPoker>(),
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, moonLordWeapons));
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // The Celestial Onion only drops if the player hasn't used one (or has not used Demon Heart in Master Mode as it alternatively functions as that there)
                    loot.AddIf((info) => (!info.player.Calamity().extraAccessoryML && !Main.masterMode) || (!info.player.extraAccessory && Main.masterMode), ModContent.ItemType<CelestialOnion>());
                    break;
                #endregion

                #region Fishing Crates
                // Kylie @ 1% (yes this is the consistent rate, dw it's not hard to get one of these)
                case ItemID.WoodenCrate:
                case ItemID.WoodenCrateHard:
                    loot.Add(ModContent.ItemType<Kylie>(), 100);
                    // We do not need to pop Hardmode ores out of Pearlwood Crates. They do not drop higher tier ores.
                    break;

                case ItemID.IronCrateHard:
                    RemoveHardmodeOresFromStandardCrates(loot);
                    loot.AddHardmodeOresToCrates(HardmodeCrateType.Mythril);
                    break;

                case ItemID.GoldenCrate:
                    RemoveBaitFromGoldenCrates(loot);
                    loot.Add(NewGoldenCrateBaitRule);
                    loot.Add(UndergroundChestLootRule);
                    break;

                case ItemID.GoldenCrateHard:
                    RemoveHardmodeOresFromStandardCrates(loot);
                    loot.AddHardmodeOresToCrates(HardmodeCrateType.Titanium);
                    RemoveBaitFromGoldenCrates(loot);
                    loot.Add(NewGoldenCrateBaitRule);
                    loot.Add(UndergroundChestLootRule);
                    break;

                // 2-5 Essences of Eleum @ 50%
                // This is our equivalent to Souls of Light/Night
                case ItemID.FrozenCrateHard:
                    RemoveHardmodeOresFromBiomeCrates(loot);
                    loot.AddHardmodeOresToCrates(HardmodeCrateType.Biome);
                    loot.Add(ModContent.ItemType<EssenceofEleum>(), 2, 2, 5);
                    break;

                // Reimplement primary loot rule to include Fledgling Wings
                case ItemID.FloatingIslandFishingCrate:
                    RemoveLootRuleFromSkyCrates(loot);
                    loot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, ItemID.ShinyRedBalloon, ItemID.LuckyHorseshoe, ItemID.Starfury, ItemID.CelestialMagnet, ItemID.CreativeWings));
                    break;

                // 2-5 Essences of Sunlight @ 50%
                // This is our equivalent to Souls of Light/Night
                case ItemID.FloatingIslandFishingCrateHard:
                    RemoveLootRuleFromSkyCrates(loot);
                    loot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, ItemID.ShinyRedBalloon, ItemID.LuckyHorseshoe, ItemID.Starfury, ItemID.CelestialMagnet, ItemID.CreativeWings));
                    RemoveHardmodeOresFromBiomeCrates(loot);
                    loot.AddHardmodeOresToCrates(HardmodeCrateType.Biome);
                    loot.Add(ModContent.ItemType<EssenceofSunlight>(), 2, 2, 5);
                    break;

                case ItemID.OasisCrate:
                    loot.Add(ModContent.ItemType<TheComb>(), 8);
                    break;

                case ItemID.OasisCrateHard:
                    RemoveHardmodeOresFromBiomeCrates(loot);
                    loot.AddHardmodeOresToCrates(HardmodeCrateType.Biome);
                    loot.Add(ModContent.ItemType<TheComb>(), 8);
                    break;

                case ItemID.CorruptFishingCrateHard:
                case ItemID.CrimsonFishingCrateHard:
                case ItemID.HallowedFishingCrateHard:
                case ItemID.DungeonFishingCrateHard:
                case ItemID.JungleFishingCrateHard:
                case ItemID.LavaCrateHard:
                case ItemID.OceanCrateHard:
                    RemoveHardmodeOresFromBiomeCrates(loot);
                    loot.AddHardmodeOresToCrates(HardmodeCrateType.Biome);
                    break;
                #endregion

                #region Miscellaneous
                // Bat Hook is now acquired from Vampires.
                case ItemID.GoodieBag:
                    RemoveBatHookFromGoodieBag(loot);
                    break;
                    #endregion
            }
        }
        #endregion

        #region Calamity Style Items From Bosses
        private static T FindRuleWithItem<T>(List<IItemDropRule> rules, int itemID, bool findLast = false) where T : class, IItemDropRule
        {
            T found = null;
            foreach (IItemDropRule rule in rules)
            {
                if (rule is T candidate && RuleContainsItem(candidate, itemID))
                {
                    if (!findLast)
                        return candidate;
                    found = candidate;
                }
            }
            return found;
        }

        private static bool RuleContainsItem<T>(T rule, int itemID) where T : IItemDropRule
        {
            if (rule is OneFromOptionsNotScaledWithLuckDropRule optionsRule)
            {
                foreach (int optionID in optionsRule.dropIds)
                    if (optionID == itemID)
                        return true;
            }

            if (rule is OneFromRulesRule rulesRule)
            {
                foreach (IItemDropRule option in rulesRule.options)
                    if (option is CommonDrop commonDrop && commonDrop.itemId == itemID)
                        return true;
            }

            if (rule is CommonDrop common)
                return common.itemId == itemID;
            if (rule is CommonDropNotScalingWithLuck commonNotScaling)
                return commonNotScaling.itemId == itemID;

            return false;
        }

        private static void RemoveDukeRules(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            IItemDropRule toRemove = RemoveRuleWithItem<CommonDropNotScalingWithLuck>(loot, rules, ItemID.FishronWings, null);
            RemoveRuleWithItem<OneFromOptionsNotScaledWithLuckDropRule>(loot, rules, ItemID.Flairon, toRemove);
        }

        private static void RemoveEmpressRules(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            IItemDropRule toRemove = RemoveRuleWithItem<CommonDropNotScalingWithLuck>(loot, rules, ItemID.RainbowWings, null);
            toRemove = RemoveRuleWithItem<OneFromOptionsNotScaledWithLuckDropRule>(loot, rules, ItemID.PiercingStarlight, toRemove);
            RemoveRuleWithItem<CommonDropNotScalingWithLuck>(loot, rules, ItemID.SparkleGuitar, toRemove);
        }

        private static IItemDropRule RemoveRuleWithItem<T>(ItemLoot loot, List<IItemDropRule> rules, int itemID, IItemDropRule previous) where T : class, IItemDropRule
        {
            IItemDropRule toRemove = FindRuleWithItem<T>(rules, itemID, findLast: true) ?? previous;
            if (toRemove is not null)
                loot.Remove(toRemove);
            return toRemove;
        }

        #endregion

        #region Fishing Crate Loot Rule Manipulation
        private static void RemoveHardmodeOresFromStandardCrates(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);

            // This is the primary rule which contains every drop
            AlwaysAtleastOneSuccessDropRule mainRule = null;
            foreach (IItemDropRule rule in rules)
                if (rule is AlwaysAtleastOneSuccessDropRule a)
                    mainRule = a;
            if (mainRule is null)
                return;

            // Find ones that are supposed to be for the ore and not the other loot
            foreach (IItemDropRule rule in mainRule.rules)
            {
                // Hardmode ores/bars are both nested within *another* nested rule
                if (rule is SequentialRulesNotScalingWithLuckRule oreRule)
                {
                    // Confirm that this is for the ore/bar then pop the numerator for the big rule
                    foreach (IItemDropRule nestedRule in oreRule.rules)
                    {
                        if (nestedRule is SequentialRulesNotScalingWithLuckRule s)
                        {
                            oreRule.chanceNumerator = 0;
                            return;
                        }
                    }
                }
            }
        }

        private static void RemoveHardmodeOresFromBiomeCrates(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);

            // This is the primary rule which contains every drop
            AlwaysAtleastOneSuccessDropRule mainRule = null;
            foreach (IItemDropRule rule in rules)
                if (rule is AlwaysAtleastOneSuccessDropRule a)
                    mainRule = a;
            if (mainRule is null)
                return;

            foreach (IItemDropRule rule in mainRule.rules)
            {
                // 2 rules, one for ore and another for bar, nested within *another* nested rule
                if (rule is SequentialRulesNotScalingWithLuckRule oreRule)
                {
                    // Confirm that this is for the ore/bar then pop the numerator for the big rule
                    foreach (IItemDropRule nestedRule in oreRule.rules)
                    {
                        if (nestedRule is OneFromRulesRule o)
                            oreRule.chanceNumerator = 0;
                    }
                }
            }
        }

        private static void RemoveBaitFromGoldenCrates(ItemLoot loot)
        {
            IItemDropRule toRemove = FindRuleWithItem<CommonDrop>(loot.Get(false), ItemID.MasterBait, findLast: true);
            if (toRemove is not null)
                loot.Remove(toRemove);
        }

        // Replaced bait rules for Golden/Titanium Crates
        // Vanilla: 3-7 Master Bait @ 66.67%
        // Calamity: 3-7 Master Bait @ 33.33% OR 3-7 Grand Marquis Bait @ 16.67%
        private static IItemDropRule NewGoldenCrateBaitRule => ItemDropRule.SequentialRulesNotScalingWithLuck(2,
                ItemDropRule.NotScalingWithLuck(ModContent.ItemType<GrandMarquisBait>(), 3, 3, 7),
                ItemDropRule.NotScalingWithLuck(ItemID.MasterBait, 1, 3, 7));

        // Non-crafted underground Gold Chest loot for Golden/Titanium Crates @ 25%; Individually 5%
        private static IItemDropRule UndergroundChestLootRule => new OneFromOptionsNotScaledWithLuckDropRule(4, 1,
                ItemID.FlareGun,
                ItemID.Mace,
                ItemID.BandofRegeneration,
                ItemID.ShoeSpikes,
                ModContent.ItemType<EnchantedKnifeStaff>()); // Climbing Claws is in Wooden/Pearlwood (vanilla) in case you're curious

        private static void RemoveLootRuleFromSkyCrates(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            AlwaysAtleastOneSuccessDropRule main = null;
            foreach (IItemDropRule rule in rules)
            {
                if (rule is AlwaysAtleastOneSuccessDropRule a)
                    main = a;
            }
            if (main is null)
                return;

            // Remove the primary loot rule and the rule for Fledgling Wings
            foreach (IItemDropRule rule in main.rules)
            {
                if (rule is OneFromOptionsNotScaledWithLuckDropRule one && one.chanceDenominator == 1)
                {
                    one.chanceNumerator = 0;
                    continue;
                }
                if (rule is CommonDrop c && c.itemId == ItemID.CreativeWings)
                {
                    c.chanceNumerator = 0;
                    continue;
                }
            }
        }
        #endregion

        #region Goodie Bag Bat Hook
        private static void RemoveBatHookFromGoodieBag(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            SequentialRulesNotScalingWithLuckRule rule1 = null;
            foreach (IItemDropRule rule in rules)
                if (rule is SequentialRulesNotScalingWithLuckRule s)
                    rule1 = s;
            if (rule1 is null)
                return;
            foreach (IItemDropRule rule in rule1.rules)
                if (rule is CommonDropNotScalingWithLuck rule2 && rule2.itemId == ItemID.BatHook)
                {
                    rule2.chanceNumerator = 0;
                    rule2.chanceDenominator = 1;
                }
        }
        #endregion
    }
}

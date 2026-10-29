using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.CalPlayer;
using CalamityMod.DataStructures;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Systems;
using CalamityMod.Systems.Collections;
using CalamityMod.UI;
using CalamityMod.UI.CalamitasEnchants;
using CalamityMod.UI.DraedonSummoning;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;

namespace CalamityMod
{
    public class ModCalls
    {
        #region Boss / Event Downed
        /// <summary>
        /// Returns whether the Calamity boss or event corresponding to the given string has been defeated.
        /// </summary>
        /// <param name="boss">The boss or event name to check. Many aliases are accepted.</param>
        /// <returns>Whether the boss or event has been defeated.</returns>
        public static bool GetBossDowned(string boss)
        {
            string normalizedBoss = boss.ToLower();
            return normalizedBoss switch
            {
                // Because this value will always be true if Acid Rain at any point in the game is beaten.
                "acid rain" or "acidrain" or "acid rain 1" or "acidrain 1" or
                    "acidrain1" or "acid rain eoc" or "acidrain eoc" or "acidraineoc" => DownedBossSystem.downedEoCAcidRain,
                "desertscourge" or "desert scourge" => DownedBossSystem.downedDesertScourge,
                "horriblehog" or "horrible hog" or "thehorriblehog" or "the horrible hog" => DownedBossSystem.downedHorribleHog,
                "clam" or "giantclam" or "giant clam" => DownedBossSystem.downedCLAM,
                "crabulon" => DownedBossSystem.downedCrabulon,
                "hivemind" or "hive mind" or "thehivemind" or "the hive mind" => DownedBossSystem.downedHiveMind,
                "perforator" or "perforators" or "theperforators" or "the perforators" or
                    "perforatorhive" or "perforator hive" or "theperforatorhive" or "the perforator hive" => DownedBossSystem.downedPerforator,
                "slimegod" or "slime god" or "theslimegod" or "the slime god" => DownedBossSystem.downedSlimeGod,
                "hmclam" or "clamhm" or "hm clam" or "clam hm" or "hmgiantclam" or "giantclamhm" or
                    "hm giant clam" or "giant clam hm" or "hardmodeclam" or "hardmode clam" or
                    "hardmodegiantclam" or "hardmode giant clam" => DownedBossSystem.downedCLAMHardMode,
                "cryogen" => DownedBossSystem.downedCryogen,
                "acid rain 2" or "acidrain 2" or "acidrain2" or "acid rain scourge" or
                    "acid rain aquatic scourge" or "acid rain aquaticscourge" or "acidrain scourge" or
                    "acidrain aquatic scourge" or "acidrain aquaticscourge" or "acidrainscourge" or
                    "acidrainaquaticscourge" => DownedBossSystem.downedAquaticScourgeAcidRain,
                "aquaticscourge" or "aquatic scourge" => DownedBossSystem.downedAquaticScourge,
                "cragmaw" or "cragmawmire" or "cragmaw mire" or "mire" => DownedBossSystem.downedCragmawMire,
                "brimstoneelemental" or "brimstone elemental" => DownedBossSystem.downedBrimstoneElemental,
                "clone" or "calamitasclone" or "calamitas clone" or "clonelamitas" or
                    "calamitasdoppelganger" or "calamitas doppelganger" => DownedBossSystem.downedCalamitasClone,
                "gss" or "greatsandshark" or "great sand shark" => DownedBossSystem.downedGSS,
                // Don't remove the old references to "Siren" here to avoid breaking other mods
                "sirenleviathan" or "siren leviathan" or "sirenandleviathan" or "siren and leviathan" or
                    "the siren and the leviathan" or "siren" or "thesiren" or "the siren" or "anahita" or
                    "leviathan" or "theleviathan" or "the leviathan" or "anahitaleviathan" or "anahita leviathan" or
                    "anahitaandleviathan" or "anahita and leviathan" or "anahita and the leviathan" => DownedBossSystem.downedLeviathan,
                "aureus" or "astrumaureus" or "astrum aureus" => DownedBossSystem.downedAstrumAureus,
                "pbg" or "plaguebringer" or "plaguebringergoliath" or "plaguebringer goliath" or
                    "theplaguebringergoliath" or "the plaguebringer goliath" => DownedBossSystem.downedPlaguebringer,
                // "scavenger" is retained for backwards compatibility.
                "scavenger" or "ravager" => DownedBossSystem.downedRavager,
                // "stargod" and "star god" are retained for backwards compatibility.
                "stargod" or "star god" or "astrumdeus" or "astrum deus" => DownedBossSystem.downedAstrumDeus,
                "guardians" or "donuts" or "profanedguardians" or "profaned guardians" or
                    "theprofanedguardians" or "the profaned guardians" => DownedBossSystem.downedGuardians,
                "dragonfolly" or "the dragonfolly" or "bumblebirb" or "bumblefuck" => DownedBossSystem.downedDragonfolly,
                "providence" or "providencetheprofanedgoddess" or "providence the profaned goddess" or
                    "providence, the profaned goddess" => DownedBossSystem.downedProvidence,
                "polterghast" or "necroghast" or "necroplasm" => DownedBossSystem.downedPolterghast,
                "mauler" => DownedBossSystem.downedMauler,
                "nuclear terror" or "nuclearterror" => DownedBossSystem.downedNuclearTerror,
                // Old Duke is also Acid Rain tier 3, so he gets those names too
                "oldduke" or "old duke" or "theoldduke" or "the old duke" or "boomerduke" or "boomer duke" or
                    "sulphurduke" or "sulphur duke" or "sulfurduke" or "sulfur duke" or "acid rain 3" or
                    "acidrain 3" or "acidrain3" or "acid rain duke" or "acidrain duke" or "acidrainduke" => DownedBossSystem.downedBoomerDuke,
                // Sentinel number aliases are retained for backwards compatibility.
                "sentinel1" or "void" or "ceaselessvoid" or "ceaseless void" => DownedBossSystem.downedCeaselessVoid,
                "sentinel2" or "stormweaver" or "storm weaver" => DownedBossSystem.downedStormWeaver,
                "sentinel3" or "cosmicwraith" or "cosmic wraith" or "signus" or
                    "signusenvoyofthedevourer" or "signus envoy of the devourer" or "signus, envoy of the devourer" => DownedBossSystem.downedSignus,
                // Sentinel aggregate aliases are retained for backwards compatibility.
                "sentinelany" or "anysentinel" or "any sentinel" or "onesentinel" or "one sentinel" or "sentinel" =>
                    DownedBossSystem.downedCeaselessVoid || DownedBossSystem.downedStormWeaver || DownedBossSystem.downedSignus,
                "sentinelall" or "sentinels" or "allsentinel" or "allsentinels" or "all sentinels" =>
                    DownedBossSystem.downedCeaselessVoid && DownedBossSystem.downedStormWeaver && DownedBossSystem.downedSignus,
                "dog" or "devourerofgods" or "devourer of gods" or "thedevourerofgods" or "the devourer of gods" => DownedBossSystem.downedDoG,
                "yharon" or "jungledragonyharon" or "jungle dragon yharon" or "jungle dragon, yharon" or
                    "yharondragonofrebirth" or "yharon, dragon of rebirth" or "yharon dragon of rebirth" or
                    "yharonresplendentphoenix" or "yharon, resplendent phoenix" or "yharon resplendent phoenix" => DownedBossSystem.downedYharon,
                "draedon" or "exomechs" or "exo mechs" or "the exo mechs" => DownedBossSystem.downedExoMechs,
                "thanatos" => DownedBossSystem.downedThanatos,
                "ares" => DownedBossSystem.downedAres,
                "exotwins" or "exo twins" or "apollo" or "artemis" => DownedBossSystem.downedArtemisAndApollo,
                "calamitas" or "scal" or "supremecalamitas" or "supreme calamitas" or
                    "supremewitchcalamitas" or "supreme witch calamitas" or "supreme witch, calamitas" => DownedBossSystem.downedCalamitas,
                "adulteidolonwyrm" or "adult eidolon wyrm" or "adultwyrm" or "adult wyrm" or
                    "adulteidolon" or "adult eidolon" or "primordialwyrm" or "primordial wyrm" => DownedBossSystem.downedPrimordialWyrm,
                "bossrush" or "boss rush" or "terminus" => DownedBossSystem.downedBossRush,
                _ => false
            };
        }
        #endregion

        #region Player in Zone / Area
        /// <summary>
        /// Returns whether the specified player is in the Calamity biome or area corresponding to the given string.
        /// </summary>
        /// <param name="p">The player whose locale is being questioned.</param>
        /// <param name="zone">The zone or area name to check. Many aliases are accepted.</param>
        /// <returns>Whether the player is currently in the zone.</returns>
        public static bool GetInZone(Player p, string zone)
        {
            CalamityPlayer mp = p.Calamity();
            string normalizedZone = zone.ToLower();
            return normalizedZone switch
            {
                // "calamity" is retained for backwards compatibility.
                // Remove the four profaned crag aliases when the actual profaned biome is added.
                "calamity" or "calamitybiome" or "calamity biome" or "crag" or "crags" or
                    "profanedcrag" or "profaned crag" or "profanedcrags" or "profaned crags" or
                    "brimstone" or "brimstonecrag" or "brimstone crag" or "brimstonecrags" or "brimstone crags" => mp.ZoneCalamity,
                "astral" or "astralbiome" or "astral biome" or "astralinfection" or "astral infection" => mp.ZoneAstral,
                "sunkensea" or "sunken sea" or "thesunkensea" or "the sunken sea" => mp.ZoneSunkenSea,
                "sulfur" or "sulphur" or "sulfursea" or "sulfur sea" or "sulphursea" or
                    "sulphur sea" or "sulfuroussea" or "sulfurous sea" or "sulphuroussea" or
                    "sulphurous sea" => mp.ZoneSulphur,
                "abyss" or "theabyss" or "the abyss" or "anyabyss" or "any abyss" or "abyssany" or "any abyss layer" => mp.ZoneAbyss,
                "abyss1" or "abyss 1" or "abyss_1" or "layer1" or "layer 1" or "layer_1" or
                    "abysslayer1" or "abyss layer 1" or "sulfur depths" or "sulfurdepths" or "sulphur depths" or
                    "sulphurdepths" or "sulfurous depths" or "sulfurousdepths" or "sulphurous depths" or
                    "sulphurousdepths" or "sulfuric depths" or "sulfuricdepths" or "sulphuric depths" or
                    "sulphuricdepths" => mp.ZoneAbyssLayer1,
                "abyss2" or "abyss 2" or "abyss_2" or "layer2" or "layer 2" or "layer_2" or
                    "abysslayer2" or "abyss layer 2" or "murky water" or "murkywater" or "murky waters" or
                    "murkywaters" => mp.ZoneAbyssLayer2,
                "abyss3" or "abyss 3" or "abyss_3" or "layer3" or "layer 3" or "layer_3" or
                    "abysslayer3" or "abyss layer 3" or "thermalvents" or "thermal vents" or "vents" => mp.ZoneAbyssLayer3,
                "abyss4" or "abyss 4" or "abyss_4" or "layer4" or "layer 4" or "layer_4" or
                    "abysslayer4" or "abyss layer 4" or "void" or "the void" or "thevoid" => mp.ZoneAbyssLayer4,
                _ => false
            };
        }
        #endregion

        #region Difficulty Modes
        /// <summary>
        /// Returns whether the Calamity difficulty modifier corresponding to the given string is currently active.
        /// </summary>
        /// <param name="difficulty">The difficulty modifier to check for.</param>
        /// <returns>Whether the difficulty is currently active.</returns>
        public static bool GetDifficultyActive(string difficulty)
        {
            string normalizedDifficulty = difficulty.ToLower();
            return normalizedDifficulty switch
            {
                "revengeance" or "rev" or "revengeancemode" or "revengeance mode" => CalamityWorld.revenge,
                "death" or "deathmode" or "death mode" => CalamityWorld.death,
                "br" or "bossrush" or "boss rush" or "bossrushactive" or "boss rush active" => BossRushEvent.BossRushActive,
                "armageddon" or "arma" or "instakill" or "instagib" or "armageddonmode" or "armageddon mode" => CalamityWorld.armageddon,
                _ => false
            };
        }

        /// <summary>
        /// Either enables or disables the Calamity difficulty modifier corresponding to the given string.<br></br>
        /// Unlike the in-game mode changing items, this has no ancillary effects such as failing if a boss is alive or instantly killing players.
        /// </summary>
        /// <param name="difficulty">The difficulty modifier to edit.</param>
        /// <param name="enabled">Whether to enable or disable the difficulty.</param>
        /// <returns></returns>
        public static bool SetDifficultyActive(string difficulty, bool enabled)
        {
            string normalizedDifficulty = difficulty.ToLower();
            return normalizedDifficulty switch
            {
                "revengeance" or "rev" or "revengeancemode" or "revengeance mode" => CalamityWorld.revenge = enabled,
                "death" or "deathmode" or "death mode" => CalamityWorld.death = enabled,
                "br" or "bossrush" or "boss rush" or "bossrushactive" or "boss rush active" => BossRushEvent.BossRushActive = enabled,
                "armageddon" or "arma" or "instakill" or "instagib" or "armageddonmode" or "armageddon mode" => CalamityWorld.armageddon = enabled,
                _ => false
            };
        }

        public static void AddCustomDifficulty(DifficultyMode newMode)
        {
            //Add the new difficulty and recalculate
            DifficultyModeSystem.Difficulties.Add(newMode);
            DifficultyModeSystem.CalculateDifficultyData();
        }

        public static void AddWorldScreenDifficulty(string name, Func<AWorldListItem, bool> function, Color color, int index = -1)
        {
            // Construct the difficulty
            WorldSelectionDifficultySystem.WorldDifficulty difficulty = new WorldSelectionDifficultySystem.WorldDifficulty(name, function, color);

            // Add the new difficulty 
            // If no index was provided, just add it to the end of the list, otherwise insert it at the desired spot on the list
            if (index == -1)
            {
                WorldSelectionDifficultySystem.WorldDifficulties.Add(difficulty);
            }
            else
            {
                WorldSelectionDifficultySystem.WorldDifficulties.Insert(index, difficulty);
            }
        }
        #endregion

        #region Rogue Class
        /// <summary>
        /// Gets a player's current rogue projectile velocity multiplier.
        /// </summary>
        /// <param name="p">The player whose rogue velocity is being queried.</param>
        /// <returns>Current rogue projectile velocity multiplier. 1f is no bonus, 2f doubles projectile speed.</returns>
        public static float GetRogueVelocity(Player p) => p?.Calamity()?.rogueVelocity ?? 1f;

        /// <summary>
        /// Adds a flat amount of rogue velocity stat to a player. This amount can be negative.
        /// </summary>
        /// <param name="p">The player whose rogue velocity is being modified.</param>
        /// <param name="add">The amount of rogue velocity to add or subtract (if negative).</param>
        /// <returns>The player's new rogue velocity stat.</returns>
        public static float AddRogueVelocity(Player p, float add) => p is null ? 1f : (p.Calamity().rogueVelocity += add);

        public static float GetCurrentStealth(Player p) => p?.Calamity()?.rogueStealth ?? 0f;

        public static float GetMaxStealth(Player p) => p?.Calamity()?.rogueStealthMax ?? 0f;

        public static float AddMaxStealth(Player p, float add) => p is null ? 0f : (p.Calamity().rogueStealthMax += add);

        public static bool CanStealthStrike(Player p) => p?.Calamity()?.StealthStrikeAvailable() ?? false;

        public static void SetStealthProjectile(Projectile projectile, bool enabled)
        {
            if (projectile != null)
                projectile.Calamity().stealthStrike = enabled;
        }

        public static bool GetStealthProjectile(Projectile projectile) => projectile?.Calamity()?.stealthStrike ?? false;

        public static void ConsumeStealth(Player player) => player?.Calamity()?.ConsumeStealthByAttacking();
        #endregion

        #region Rippers
        public static float GetRage(Player p) => p?.Calamity()?.rage ?? 0;
        public static float GetAdrenaline(Player p) => p?.Calamity()?.adrenaline ?? 0;
        public static float GetRageMax(Player p) => p?.Calamity()?.rageMax ?? 0;
        public static float GetAdrenalineMax(Player p) => p?.Calamity()?.adrenalineMax ?? 0;
        #endregion

        #region Charge
        public static bool GetChargeable(Item i) => i?.Calamity()?.UsesCharge ?? false;
        public static void SetChargeable(Item i, bool chargeable)
        {
            if (i != null)
                i.Calamity().UsesCharge = chargeable;
        }
        public static float GetCharge(Item i) => i?.Calamity()?.Charge ?? 0;
        public static void SetCharge(Item i, float charge)
        {
            if (i != null)
                i.Calamity().Charge = charge;
        }
        public static float GetMaxCharge(Item i) => i?.Calamity()?.MaxCharge ?? 0;
        public static void SetMaxCharge(Item i, float chargeMax)
        {
            if (i != null)
                i.Calamity().MaxCharge = chargeMax;
        }
        public static float GetChargePerUse(Item i) => i?.Calamity()?.ChargePerUse ?? 0;
        public static void SetChargePerUse(Item i, float chargeUse)
        {
            if (i != null)
                i.Calamity().ChargePerUse = chargeUse;
        }
        public static float GetChargePerAltUse(Item i) => i?.Calamity()?.ChargePerAltUse ?? 0;
        public static void SetChargePerAltUse(Item i, float chargeAltUse)
        {
            if (i != null)
                i.Calamity().ChargePerAltUse = chargeAltUse;
        }
        #endregion

        #region Mouse Listening
        public static bool GetRightClickListener(Player p) => p?.Calamity()?.rightClickListener ?? false;
        public static bool GetMouseWorldListener(Player p) => p?.Calamity()?.mouseWorldListener ?? false;
        public static bool GetRightClick(Player p) => p?.Calamity()?.mouseRight ?? false;
        public static Vector2 GetMouseWorld(Player p) => p?.Calamity()?.mouseWorld ?? Vector2.Zero;
        public static void SetRightClickListener(Player p, bool active)
        {
            if (p != null)
                p.Calamity().rightClickListener = active;
        }
        public static void SetMouseWorldListener(Player p, bool active)
        {
            if (p != null)
                p.Calamity().mouseWorldListener = active;
        }
        #endregion

        #region Other Player Stats
        public static float GetDarknessIntensity(Player p)
        {
            if (p != null)
                return p.Calamity().darknessIntensity;
            return 0;
        }

        public static void AddAbyssLightStrength(Player p, float add)
        {
            if (p != null)
                p.Calamity().abyssDarkness -= add;
        }

        public static void AddBreathTick(Player p, float add)
        {
            if (p != null)
                p.Calamity().externalBreathTickBoost += add;
        }

        public static void AddFlightTimeMult(Player p, float add)
        {
            if (p != null)
                p.Calamity().externalFlightTimeMultBoost += add;
        }

        public static void ToggleInfiniteFlight(Player p, bool enabled)
        {
            if (p != null)
                p.Calamity().infiniteFlight = enabled;
        }

        public static bool GetWearingRogueArmor(Player p) => p?.Calamity()?.wearingRogueArmor ?? false;

        public static void SetWearingRogueArmor(Player p, bool enabled)
        {
            if (p != null)
                p.Calamity().wearingRogueArmor = enabled;
        }

        public static bool GetWearingPostMLSummonerArmor(Player p) => p?.Calamity()?.WearingPostMLSummonerSet ?? false;

        public static void SetWearingPostMLSummonerArmor(Player p, bool enabled)
        {
            if (p != null)
                p.Calamity().WearingPostMLSummonerSet = enabled;
        }

        public static bool SetPlayerColdImmune(Player p, bool cold) => p is not null && (p.Calamity().externalColdImmunity = cold);
        public static bool SetPlayerHeatImmune(Player p, bool heat) => p is not null && (p.Calamity().externalHeatImmunity = heat);
        public static bool SetPlayerDefenseDamageImmune(Player p, bool dd) => p is not null && (p.Calamity().externalDefenseDamageImmunity = dd);
        #endregion

        #region NPC Damage Reduction
        // Sets the damage reduction for an NPC type
        public static float SetDamageReduction(int npcID, float dr)
        {
            CalamityGlobalNPC.DRValues.TryGetValue(npcID, out float oldDR);
            CalamityGlobalNPC.DRValues.Remove(npcID);
            CalamityGlobalNPC.DRValues.Add(npcID, dr);
            return oldDR;
        }
        // Sets a specific NPC's damage reduction
        public static void SetDamageReductionSpecific(NPC npc, float dr)
        {
            if (npc != null)
                npc.Calamity().DR = dr;
        }
        // Gets a specific NPC's current damage reduction
        public static float GetDamageReduction(NPC npc) => npc?.Calamity()?.DR ?? 0f;
        #endregion

        #region Defense Damage
        // Allow an NPC to deal defense damage
        public static void SetDefenseDamageNPC(NPC npc, bool enabled)
        {
            if (npc != null)
                npc.Calamity().canBreakPlayerDefense = enabled;
        }
        // Gets if an NPC can deal defense damage
        public static bool GetDefenseDamageNPC(NPC npc) => npc?.Calamity()?.canBreakPlayerDefense ?? false;
        // Allow a projectile to deal defense damage
        public static void SetDefenseDamageProjectile(Projectile projectile, bool enabled)
        {
            if (projectile != null)
                projectile.Calamity().DealsDefenseDamage = enabled;
        }
        // Gets if a projectile can deal defense damage
        public static bool GetDefenseDamageProjectile(Projectile projectile) => projectile?.Calamity()?.DealsDefenseDamage ?? false;
        #endregion

        #region Debuff Vulnerabilities
        public static void SetDebuffVulnerability(NPC npc, string debuffName, bool? enabled)
        {
            if (npc != null)
            {
                switch (debuffName.ToLower())
                {
                    case "cold":
                    case "ice":
                    case "frozen":
                    case "freezing":
                        npc.Calamity().VulnerableToCold = enabled;
                        break;

                    case "electricity":
                    case "electric":
                    case "lightning":
                    case "thunder":
                        npc.Calamity().VulnerableToElectricity = enabled;
                        break;

                    case "heat":
                    case "hot":
                    case "fire":
                    case "burning":
                        npc.Calamity().VulnerableToHeat = enabled;
                        break;

                    case "sickness":
                    case "sick":
                    case "poison":
                    case "poisoned":
                    case "venom":
                        npc.Calamity().VulnerableToSickness = enabled;
                        break;

                    case "water":
                    case "wet":
                    case "drown":
                    case "drowning":
                        npc.Calamity().VulnerableToWater = enabled;
                        break;
                }
            }
        }
        public static bool? GetDebuffVulnerability(NPC npc, string debuffName)
        {
            if (npc != null)
            {
                switch (debuffName.ToLower())
                {
                    default:
                        return false;

                    case "cold":
                    case "ice":
                    case "frozen":
                    case "freezing":
                        return npc?.Calamity()?.VulnerableToCold ?? null;

                    case "electricity":
                    case "electric":
                    case "lightning":
                    case "thunder":
                        return npc?.Calamity()?.VulnerableToElectricity ?? null;

                    case "heat":
                    case "hot":
                    case "fire":
                    case "burning":
                        return npc?.Calamity()?.VulnerableToHeat ?? null;

                    case "sickness":
                    case "sick":
                    case "poison":
                    case "poisoned":
                    case "venom":
                        return npc?.Calamity()?.VulnerableToSickness ?? null;

                    case "water":
                    case "wet":
                    case "drown":
                    case "drowning":
                        return npc?.Calamity()?.VulnerableToWater ?? null;
                }
            }
            return false;
        }
        #endregion

        #region Calamity AI

        public static float[] GetCalamityAI(NPC npc) => npc?.Calamity()?.newAI ?? new float[0];

        public static void SetCalamityAI(NPC npc, int aiSlot, float value)
        {
            if (npc != null)
            {
                npc.Calamity().newAI[aiSlot] = value;
            }
        }
        #endregion

        #region Boss Health Bars
        public static bool BossHealthBarVisible() => Main.LocalPlayer.Calamity().drawBossHPBar;

        public static bool SetBossHealthBarVisible(bool visible) => Main.LocalPlayer.Calamity().drawBossHPBar = visible;

        public static bool GetShouldCloseBossHealthBar(NPC npc) => npc?.Calamity()?.ShouldCloseHPBar ?? false;
        public static void SetShouldCloseBossHealthBar(NPC npc, bool enabled) => npc.Calamity().ShouldCloseHPBar = enabled;
        #endregion

        #region Dodge Disabling
        public static bool AreDodgesDisabled() => Main.LocalPlayer.Calamity().disableAllDodges;

        public static bool DisableAllDodges(bool disable) => Main.LocalPlayer.Calamity().disableAllDodges = disable;
        #endregion

        #region Debuff Display support
        public static void RegisterDebuff(string texturePath, Predicate<NPC> debuffCheck)
        {
            if (!CalamityGlobalNPC.moddedDebuffTextureList.Contains((texturePath, debuffCheck)))
            {
                CalamityGlobalNPC.moddedDebuffTextureList.Add((texturePath, debuffCheck));
            }
        }
        #endregion

        #region Town NPC Alert support
        public static void RegisterTownNPCShop(int id, Predicate<Player> getShop, Action<Player, bool> setShop)
        {
            if (!CalamityGlobalTownNPC.npcAlertList.Contains((id, getShop, setShop)))
            {
                CalamityGlobalTownNPC.npcAlertList.Add((id, getShop, setShop));
            }
        }
        #endregion

        #region Permanent Boosters
        // [Aliases("GetPermanentPowerup", "GetPowerup", "HasPermanentBooster", "GetPermanentBooster", "GetBooster")]
        public static bool HasPermanentPowerup(Player player, string powerupName)
        {
            return powerupName switch
            {
                "SanguineTangerine" => player.Calamity().sTangerine,
                "MiracleFruit" => player.Calamity().mFruit,
                "TaintedCloudberry" => player.Calamity().tCloudberry,
                "SacredStrawberry" => player.Calamity().sStrawberry,

                "CometShard" => player.Calamity().cShard,
                "EtherealCore" => player.Calamity().eCore,
                "PhantomHeart" => player.Calamity().pHeart,

                "MushroomPlasmaRoot" => player.Calamity().rageBoostOne,
                "InfernalBlood" => player.Calamity().rageBoostTwo,
                "RedLightningContainer" => player.Calamity().rageBoostThree,

                "ElectrolyteGelPack" => player.Calamity().adrenalineBoostOne,
                "StarlightFuelCell" => player.Calamity().adrenalineBoostTwo,
                "Ectoheart" => player.Calamity().adrenalineBoostThree,

                "CelestialOnion" => player.Calamity().extraAccessoryML,

                _ => false, // Return false if no case is found
            };
        }

        // [Aliases("SetPowerup", "SetPermanentBooster", "SetBooster")]
        public static void SetPermanentPowerup(Player player, string powerupName, bool value)
        {

            switch (powerupName)
            {
                case "SanguineTangerine": player.Calamity().sTangerine = value; break;
                case "MiracleFruit": player.Calamity().mFruit = value; break;
                case "TaintedCloudberry": player.Calamity().tCloudberry = value; break;
                case "SacredStrawberry": player.Calamity().sStrawberry = value; break;

                case "CometShard": player.Calamity().cShard = value; break;
                case "EtherealCore": player.Calamity().eCore = value; break;
                case "PhantomHeart": player.Calamity().pHeart = value; break;

                case "MushroomPlasmaRoot": player.Calamity().rageBoostOne = value; break;
                case "InfernalBlood": player.Calamity().rageBoostTwo = value; break;
                case "RedLightningContainer": player.Calamity().rageBoostThree = value; break;

                case "ElectrolyteGelPack": player.Calamity().adrenalineBoostOne = value; break;
                case "StarlightFuelCell": player.Calamity().adrenalineBoostTwo = value; break;
                case "Ectoheart": player.Calamity().adrenalineBoostThree = value; break;

                case "CelestialOnion": player.Calamity().extraAccessoryML = value; break;
            }
        }
        #endregion

        #region Call

        public static object Call(params object[] args)
        {
            bool isValidPlayerArg(object o) => o is int || o is Player;
            bool isValidItemArg(object o) => o is int || o is Item;
            bool isValidProjectileArg(object o) => o is int || o is Projectile;
            bool isValidNPCArg(object o) => o is int || o is NPC;

            Player castPlayer(object o)
            {
                if (o is int i)
                    return Main.player[i];
                else if (o is Player p)
                    return p;
                return null;
            }

            Item castItem(object o)
            {
                if (o is int i)
                    return Main.item[i];
                else if (o is Item it)
                    return it;
                return null;
            }

            Projectile castProjectile(object o)
            {
                if (o is int i)
                    return Main.projectile[i];
                else if (o is Projectile p)
                    return p;
                return null;
            }

            NPC castNPC(object o)
            {
                if (o is int i)
                    return Main.npc[i];
                else if (o is NPC n)
                    return n;
                return null;
            }

            // Certain IDs in vanilla's files are shorts instead of ints for some reason.
            // Instead of expecting developers to have to manually cast these IDs, this function is used
            // to handle IDs that are either ints OR shorts, without the worry of missing a cast and wondering why
            // the Mod Call did nothing.
            bool castID(object o, out int id)
            {
                id = -1;
                if (!(o is int) && !(o is short))
                    return false;

                if (o is short shortID)
                    id = shortID;
                if (o is int intID)
                    id = intID;

                return true;
            }

            if (args is null || args.Length <= 0)
                return new ArgumentNullException("ERROR: No function name specified. First argument must be a function name.");
            if (!(args[0] is string))
                return new ArgumentException("ERROR: First argument must be a string function name.");

            string methodName = args[0].ToString();
            switch (methodName)
            {
                case "Downed":
                case "GetDowned":
                case "BossDowned":
                case "GetBossDowned":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a boss or event name as a string.");
                    if (!(args[1] is string))
                        return new ArgumentException("ERROR: The argument to \"Downed\" must be a string.");
                    return GetBossDowned(args[1].ToString());

                case "Zone":
                case "GetZone":
                case "InZone":
                case "GetInZone":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and a zone name as a string.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify a zone name as a string.");
                    if (!(args[2] is string))
                        return new ArgumentException("ERROR: The second argument to \"InZone\" must be a string.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"InZone\" must be a Player or an int.");
                    return GetInZone(castPlayer(args[1]), args[2].ToString());

                case "Difficulty":
                case "GetDifficulty":
                case "DifficultyActive":
                case "GetDifficultyActive":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a difficulty mode name as a string.");
                    if (!(args[1] is string))
                        return new ArgumentException("ERROR: The argument to \"Difficulty\" must be a string.");
                    return GetDifficultyActive(args[1].ToString());

                case "SetDifficulty":
                case "SetDifficultyActive":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a difficulty mode name as a string and a bool.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify a bool.");
                    if (!(args[2] is bool enabled))
                        return new ArgumentException("ERROR: The second argument to \"SetDifficulty\" must be a bool.");
                    if (!(args[1] is string))
                        return new ArgumentException("ERROR: The first argument to \"SetDifficulty\" must be a string.");
                    return SetDifficultyActive(args[1].ToString(), enabled);


                case "AddDifficultyToUI":
                    if (args.Length < 2)
                        return new ArgumentException("ERROR: Not enough arguements provided");

                    if (args[1] is not DifficultyMode mode)
                        return new ArgumentException("ERROR: A class inheriting from 'DifficultyMode' must be provided.");
                    AddCustomDifficulty(mode);
                    return null;

                case "AddWorldScreenDifficulty":
                case "AddWorldSelectionDifficulty":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify a difficulty mode name as a string.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify a Func<AWorldListItem, bool>.");
                        if (args.Length < 4)
                            return new ArgumentNullException("ERROR: Must specify a Color.");

                        if (args.Length >= 5)
                        {
                            if (!(args[4] is int))
                                return new ArgumentException("ERROR: The fourth argument to \"AddWorldScreenDifficulty\" must be an int.");
                        }
                        if (!(args[3] is Color color))
                            return new ArgumentException("ERROR: The third argument to \"AddWorldScreenDifficulty\" must be a Color.");
                        if (!(args[2] is Func<AWorldListItem, bool> worldFunction))
                            return new ArgumentException("ERROR: The second argument to \"AddWorldScreenDifficulty\" must be a Func<AWorldListItem, bool>.");
                        if (!(args[1] is string))
                            return new ArgumentException("ERROR: The first argument to \"AddWorldScreenDifficulty\" must be a string.");

                        if (args.Length >= 5)
                            AddWorldScreenDifficulty(args[1].ToString(), worldFunction, color, (int)args[4]);
                        else
                            AddWorldScreenDifficulty(args[1].ToString(), worldFunction, color);
                        return null;
                    }

                case "GetDarkness":
                case "GetAbyssDarkness":
                case "GetDarknessIntensity":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The argument to \"GetLightStrength\" must be a Player or an int.");
                    return GetDarknessIntensity(castPlayer(args[1]));

                case "AddLight":
                case "AddLightLevel":
                case "AddLightStrength":
                case "AddAbyssLight":
                case "AddAbyssLightLevel":
                case "AddAbyssLightStrength":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and light strength change as an int.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify light strength change as an int.");
                    if (!(args[2] is int light))
                        return new ArgumentException("ERROR: The second argument to \"AddLightStrength\" must be an int.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"AddLightStrength\" must be a Player or an int.");
                    AddAbyssLightStrength(castPlayer(args[1]), light);
                    return null;

                case "BreathTick":
                case "AbyssBreathTick":
                case "AddBreathTick":
                case "AddAbyssBreathTick":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and Abyss breath tick change as a float.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify Abyss breath tick change as a float.");
                    if (!(args[2] is float breathTick))
                        return new ArgumentException("ERROR: The second argument to \"AddBreathTick\" must be a float.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"AddBreathTick\" must be a Player or an int.");
                    AddBreathTick(castPlayer(args[1]), breathTick);
                    return null;

                case "FlightMult":
                case "FlightTimeMult":
                case "AddFlightMult":
                case "AddFlightTimeMult":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and flight mult change as a float.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify flight mult change as a float.");
                    if (!(args[2] is float flightMult))
                        return new ArgumentException("ERROR: The second argument to \"AddFlightMult\" must be a float.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"AddFlightMult\" must be a Player or an int.");
                    AddFlightTimeMult(castPlayer(args[1]), flightMult);
                    return null;

                case "InfiniteFlight":
                case "AddInfiniteFlight":
                case "EnableInfiniteFlight":
                case "ToggleInfiniteFlight":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and if the player should gain infinite flight as a bool.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify if a player should gain infinite flight as a bool.");
                    if (!(args[2] is bool))
                        return new ArgumentException("ERROR: The second argument to \"InfiniteFlight\" must be a bool.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"InfiniteFlight\" must be a Player or an int.");
                    bool fly = (bool)args[2];
                    ToggleInfiniteFlight(castPlayer(args[1]), fly);
                    return null;

                case "GetRogueArmor":
                case "GetWearingRogueArmor":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The argument to \"GetRogueArmor\" must be a Player or an int.");
                    return GetWearingRogueArmor(castPlayer(args[1]));

                case "SetRogueArmor":
                case "SetWearingRogueArmor":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and if the player should be counted as wearing rogue armor as a bool.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify if a player should be counted as wearing rogue armor as a bool.");
                    if (!(args[2] is bool))
                        return new ArgumentException("ERROR: The second argument to \"SetRogueArmor\" must be a bool.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"SetRogueArmor\" must be a Player or an int.");
                    bool roguearmor = (bool)args[2];
                    SetWearingRogueArmor(castPlayer(args[1]), roguearmor);
                    return null;

                case "GetPostMLSummonArmor":
                case "GetWearingPostMLSummonArmor":
                case "GetPostMoonLordSummonArmor":
                case "GetWearingPostMoonLordSummonArmor":
                case "GetPostMLSummonerArmor":
                case "GetWearingPostMLSummonerArmor":
                case "GetPostMoonLordSummonerArmor":
                case "GetWearingPostMoonLordSummonerArmor":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The argument to \"GetPostMoonLordSummonerArmor\" must be a Player or an int.");
                    return GetWearingPostMLSummonerArmor(castPlayer(args[1]));

                case "SetPostMLSummonArmor":
                case "SetWearingPostMLSummonArmor":
                case "SetPostMoonLordSummonArmor":
                case "SetWearingPostMoonLordSummonArmor":
                case "SetPostMLSummonerArmor":
                case "SetWearingPostMLSummonerArmor":
                case "SetPostMoonLordSummonerArmor":
                case "SetWearingPostMoonLordSummonerArmor":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and if the player should be counted as wearing Post-Moon Lord summoner armor as a bool.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify if a player should be counted as wearing Post-Moon Lord summoner armor as a bool.");
                    if (!(args[2] is bool))
                        return new ArgumentException("ERROR: The second argument to \"SetPostMoonLordSummonerArmor\" must be a bool.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"SetPostMoonLordSummonerArmor\" must be a Player or an int.");
                    bool summonarmor = (bool)args[2];
                    SetWearingPostMLSummonerArmor(castPlayer(args[1]), summonarmor);
                    return null;

                case "GetRogueVelocity":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The argument to \"GetRogueVelocity\" must be a Player or an int.");
                    return GetRogueVelocity(castPlayer(args[1]));

                case "AddRogueVelocity":
                case "ModifyRogueVelocity":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and rogue velocity change as a float.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify rogue velocity change as a float.");
                    if (!(args[2] is float velocity))
                        return new ArgumentException("ERROR: The second argument to \"AddRogueVelocity\" must be a float.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"AddRogueVelocity\" must be a Player or an int.");
                    return AddRogueVelocity(castPlayer(args[1]), velocity);

                case "GetStealth":
                case "GetCurrentStealth":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetStealth\" must be a Player or an int.");
                    return GetCurrentStealth(castPlayer(args[1]));

                case "GetMaxStealth":
                case "GetStealthCap":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetMaxStealth\" must be a Player or an int.");
                    return GetMaxStealth(castPlayer(args[1]));

                case "AddMaxStealth":
                case "ModifyMaxStealth":
                case "ModifyStealthCap":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and rogue max stealth as a float.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify rogue max stealth as a float.");
                    if (!(args[2] is float maxStealth))
                        return new ArgumentException("ERROR: The second argument to \"AddMaxStealth\" must be a float.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"AddMaxStealth\" must be a Player or an int.");
                    return AddMaxStealth(castPlayer(args[1]), maxStealth);

                case "CanStealthStrike":
                case "StealthStrikeAvailable":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"CanStealthStrike\" must be a Player or an int.");
                    return CanStealthStrike(castPlayer(args[1]));

                case "SetStealthProjectile":
                case "SetStealthStrikeProjectile":
                case "SetProjectileStealth":
                case "SetProjectileStealthStrike":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both a Projectile and if the Projectile should be counted as a stealth strike as a bool.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify whether or not the projectile was created from a stealth strike as a bool.");
                        if (!(args[2] is bool))
                            return new ArgumentException("ERROR: The second argument to \"SetStealthProjectile\" must be a bool.");
                        if (!isValidProjectileArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetStealthProjectile\" must be a Projectile.");

                        bool ssEnabled = (bool)args[2];
                        SetStealthProjectile(castProjectile(args[1]), ssEnabled);
                        return null;
                    }

                case "GetStealthProjectile":
                case "GetStealthStrikeProjectile":
                case "GetProjectileStealth":
                case "GetProjectileStealthStrike":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify a Projectile.");
                        if (!isValidProjectileArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"GetStealthProjectile\" must be a Projectile.");
                        return GetStealthProjectile(castProjectile(args[1]));
                    }

                case "ConsumeStealth":
                case "ConsumeStealthByAttacking":
                case "UseStealth":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                        if (!isValidPlayerArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"ConsumeStealth\" must be a Player or an int.");
                        ConsumeStealth(castPlayer(args[1]));
                        return null;
                    }

                case "GetRage":
                case "GetRageCurrent":
                case "GetCurrentRage":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetRage\" must be a Player or an int.");
                    return GetRage(castPlayer(args[1]));

                case "GetAdrenaline":
                case "GetAdrenalineCurrent":
                case "GetCurrentAdrenaline":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetAdrenaline\" must be a Player or an int.");
                    return GetAdrenaline(castPlayer(args[1]));

                case "GetMaxRage":
                case "GetRageMax":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetMaxRage\" must be a Player or an int.");
                    return GetRageMax(castPlayer(args[1]));

                case "GetMaxAdrenaline":
                case "GetAdrenalineMax":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetMaxAdrenaline\" must be a Player or an int.");
                    return GetAdrenalineMax(castPlayer(args[1]));

                case "GetMaxCharge":
                case "GetChargeMax":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify an Item object (or int index of an Item).");
                    if (!isValidItemArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetMaxCharge\" must be an Item or an int.");
                    return GetMaxCharge(castItem(args[1]));

                case "SetMaxCharge":
                case "SetChargeMax":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both an Item and charge as a float or double.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify charge as a float or double.");
                        if (!(args[2] is float) && !(args[2] is double))
                            return new ArgumentException("ERROR: The second argument to \"SetMaxCharge\" must be a float or a double.");
                        if (!isValidItemArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetMaxCharge\" must be an Item.");

                        float Charge = (float)args[2];
                        SetMaxCharge(castItem(args[1]), Charge);
                        return null;
                    }

                case "GetCharge":
                case "GetCurrentCharge":
                case "GetChargeCurrent":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify an Item object (or int index of an Item).");
                    if (!isValidItemArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetCharge\" must be an Item or an int.");
                    return GetCharge(castItem(args[1]));

                case "SetCharge":
                case "SetCurrentCharge":
                case "SetChargeCurrent":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both an Item and charge as a float or double.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify charge as a float or double.");
                        if (!(args[2] is float) && !(args[2] is double))
                            return new ArgumentException("ERROR: The second argument to \"SetCharge\" must be a float or a double.");
                        if (!isValidItemArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetCharge\" must be an Item.");

                        float Charge = (float)args[2];
                        SetCharge(castItem(args[1]), Charge);
                        return null;
                    }

                case "GetChargePerUse":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify an Item object (or int index of an Item).");
                    if (!isValidItemArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetChargePerUse\" must be an Item or an int.");
                    return GetChargePerUse(castItem(args[1]));

                case "SetChargePerUse":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both an Item and charge as a float or double.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify charge as a float or double.");
                        if (!(args[2] is float) && !(args[2] is double))
                            return new ArgumentException("ERROR: The second argument to \"SetChargePerUse\" must be a float or a double.");
                        if (!isValidItemArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetChargePerUse\" must be an Item.");

                        float Charge = (float)args[2];
                        SetChargePerUse(castItem(args[1]), Charge);
                        return null;
                    }

                case "GetChargePerAltUse":
                case "GetChargePerUseAlt":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify an Item object (or int index of an Item).");
                    if (!isValidItemArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetChargePerUse\" must be an Item or an int.");
                    return GetChargePerAltUse(castItem(args[1]));

                case "SetChargePerAltUse":
                case "SetChargeUseAlt":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both an Item and charge as a float or double.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify charge as a float or double.");
                        if (!(args[2] is float) && !(args[2] is double))
                            return new ArgumentException("ERROR: The second argument to \"SetChargePerUseAlt\" must be a float or a double.");
                        if (!isValidItemArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetChargePerUseAlt\" must be an Item.");

                        float Charge = (float)args[2];
                        SetChargePerAltUse(castItem(args[1]), Charge);
                        return null;
                    }

                case "GetChargeable":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify an Item object (or int index of an Item).");
                    if (!isValidItemArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetChargeable\" must be an Item or an int.");
                    return GetChargeable(castItem(args[1]));

                case "SetChargeable":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both an Item and if the item can be charged as a bool.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify the ability to charge as a bool.");
                        if (!(args[2] is bool))
                            return new ArgumentException("ERROR: The second argument to \"SetChargeable\" must be a bool.");
                        if (!isValidItemArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetChargeable\" must be an Item.");

                        bool Charge = (bool)args[2];
                        SetChargeable(castItem(args[1]), Charge);
                        return null;
                    }

                case "GetRightClickListener":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetRightClickListener\" must be a Player or an int.");
                    return GetRightClickListener(castPlayer(args[1]));

                case "GetMouseWorldListener":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetMouseWorldListener\" must be a Player or an int.");
                    return GetMouseWorldListener(castPlayer(args[1]));

                case "GetRightClick":
                case "GetMouseRight":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetRightClick\" must be a Player or an int.");
                    return GetRightClick(castPlayer(args[1]));

                case "GetMouseWorld":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify a Player object (or int index of a Player).");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"GetMouseWorld\" must be a Player or an int.");
                    return GetMouseWorld(castPlayer(args[1]));

                case "SetRightClickListener":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both a Player and whether or not the listener is active.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify status of the listener as a bool.");
                        if (!(args[2] is bool))
                            return new ArgumentException("ERROR: The second argument to \"SetRightClickListener\" must be a bool.");
                        if (!isValidPlayerArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetRightClickListener\" must be a Player.");

                        bool active = (bool)args[2];
                        SetRightClickListener(castPlayer(args[1]), active);
                        return null;
                    }

                case "SetMouseWorldListener":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both a Player and whether or not the listener is active.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify status of the listener as a bool.");
                        if (!(args[2] is bool))
                            return new ArgumentException("ERROR: The second argument to \"SetMouseWorldListener\" must be a bool.");
                        if (!isValidPlayerArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetMouseWorldListener\" must be a Player.");

                        bool active = (bool)args[2];
                        SetMouseWorldListener(castPlayer(args[1]), active);
                        return null;
                    }

                case "SetDR":
                case "SetDamageReduction":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both NPC ID as an int and damage reduction as a float or double.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify damage reduction as a float or double.");
                        if (!(args[2] is float) && !(args[2] is double))
                            return new ArgumentException("ERROR: The second argument to \"SetDamageReduction\" must be a float or a double.");
                        if (!castID(args[1], out int npcID))
                            return new ArgumentException("ERROR: The first argument to \"SetDamageReduction\" must be an int or short ID.");

                        float DR = (float)args[2];
                        return SetDamageReduction(npcID, DR);
                    }

                case "SetDRSpecific":
                case "SetDamageReductionSpecfic":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both an NPC and damage reduction as a float or double.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify damage reduction as a float or double.");
                        if (!(args[2] is float) && !(args[2] is double))
                            return new ArgumentException("ERROR: The second argument to \"SetDamageReduction\" must be a float or a double.");
                        if (!isValidNPCArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetDamageReduction\" must be an NPC.");

                        float DR = (float)args[2];
                        SetDamageReductionSpecific(castNPC(args[1]), DR);
                        return null;
                    }

                case "GetDamageReduction":
                case "GetDR":
                case "GetDRSpecific":
                case "GetDamageReductionSpecific":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify an NPC.");
                        if (!isValidNPCArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"GetDamageReduction\" must be an NPC.");
                        return GetDamageReduction(castNPC(args[1]));
                    }

                case "SetDefenseDamageNPC":
                case "SetNPCDefenseDamage":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both an NPC and if the NPC can deal defense damage as a bool.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify the ability to deal defense damage as a bool.");
                        if (!(args[2] is bool))
                            return new ArgumentException("ERROR: The second argument to \"SetDefenseDamageNPC\" must be a bool.");
                        if (!isValidNPCArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetDefenseDamageNPC\" must be an NPC.");

                        bool ddEnabled = (bool)args[2];
                        SetDefenseDamageNPC(castNPC(args[1]), ddEnabled);
                        return null;
                    }

                case "GetDefenseDamageNPC":
                case "GetNPCDefenseDamage":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify an NPC.");
                        if (!isValidNPCArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"GetDefenseDamageNPC\" must be an NPC.");
                        return GetDefenseDamageNPC(castNPC(args[1]));
                    }

                case "SetDefenseDamageProjectile":
                case "SetProjectileDefenseDamage":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both a Projectile and if the Projectile can deal defense damage as a bool.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify the ability to deal defense damage as a bool.");
                        if (!(args[2] is bool))
                            return new ArgumentException("ERROR: The second argument to \"SetDefenseDamageProjectile\" must be a bool.");
                        if (!isValidProjectileArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetDefenseDamageProjectile\" must be a Projectile.");

                        bool ddEnabled = (bool)args[2];
                        SetDefenseDamageProjectile(castProjectile(args[1]), ddEnabled);
                        return null;
                    }

                case "GetDefenseDamageProjectile":
                case "GetProjectileDefenseDamage":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify a Projectile.");
                        if (!isValidNPCArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"GetDefenseDamageProjectile\" must be a Projectile.");
                        return GetDefenseDamageProjectile(castProjectile(args[1]));
                    }

                case "GetDebuffVulnerability":
                case "GetDebuffVulnerabilities":
                case "GetVulnerableDebuffs":
                case "GetVulnerability":
                case "GetVulnerabilities":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify an NPC and a debuff type as a string.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify a debuff type as a string.");
                        if (!(args[2] is string))
                            return new ArgumentException("ERROR: The second argument to \"SetDebuffVulnerability\" must be a string.");
                        if (!isValidNPCArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetDebuffVulnerability\" must be an NPC.");
                        return GetDebuffVulnerability(castNPC(args[1]), args[2].ToString());
                    }

                case "SetDebuffVulnerability":
                case "SetDebuffVulnerabilities":
                case "SetVulnerableDebuffs":
                case "SetVulnerability":
                case "SetVulnerabilities":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify an NPC, debuff type as a string, and whether to add or remove a vulnerability as a bool.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify a debuff type as a string, and whether to add or remove a vulnerability as a bool.");
                        if (args.Length < 4)
                            return new ArgumentNullException("ERROR: Must specify whether to add or remove a vulnerability as a bool.");
                        if ((!(args[3] is bool)) && args[3] != null)
                            return new ArgumentException("ERROR: The third argument to \"SetDebuffVulnerability\" must be a bool.");
                        if (!(args[2] is string))
                            return new ArgumentException("ERROR: The second argument to \"SetDebuffVulnerability\" must be a string.");
                        if (!isValidNPCArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetDebuffVulnerability\" must be an NPC.");
                        SetDebuffVulnerability(castNPC(args[1]), args[2].ToString(), (bool?)args[3]);
                        return null;
                    }

                case "GetVanillaAIOverrideEnabled":
                    {
                        return CalamityVanillaAIOverrideNPC.Enabled;
                    }

                case "SetVanillaAIOverrideEnabled":
                    {
                        if (args.Length < 1)
                            return new ArgumentNullException(nameof(args), "ERROR: Must specify a bool parameter");
                        if (args[1] is not bool aiOverrideEnabled)
                            return new ArgumentException("ERROR: The third argument to \"SetVanillaAIOverrideEnabled\" must be a bool.");
                        CalamityVanillaAIOverrideNPC.Enabled = aiOverrideEnabled;
                        return null;
                    }

                case "GetCalamityAI":
                case "GetNewAI":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify an NPC.");
                        if (!isValidNPCArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"GetCalamityAI\" must be an NPC.");
                        return GetCalamityAI(castNPC(args[1]));
                    }

                case "SetCalamityAI":
                case "SetNewAI":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify an NPC, an AI slot as an int, and a value for it as a float or a double.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify an AI slot as an int, and a value for it as a float or a double.");
                        if (args.Length < 4)
                            return new ArgumentNullException("ERROR: Must specify a value for the AI slot as a float or a double.");
                        if (!(args[3] is float) && !(args[3] is double))
                            return new ArgumentException("ERROR: The third argument to \"SetCalamityAI\" must be a float or a double.");
                        if (!(args[2] is int newValue))
                            return new ArgumentException("ERROR: The second argument to \"SetCalamityAI\" must be an int.");
                        if (!isValidNPCArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetCalamityAI\" must be an NPC.");
                        SetCalamityAI(castNPC(args[1]), newValue, (float)args[3]);
                        return null;
                    }

                case "BossHealthBarVisible":
                case "BossHealthBarsVisible":
                case "GetBossHealthBarVisible":
                case "GetBossHealthBarsVisible":
                    return BossHealthBarVisible();

                case "SetBossHealthBarVisible":
                case "SetBossHealthBarsVisible":
                    if (args.Length < 2 || !(args[1] is bool bossBarEnabled))
                        return new ArgumentNullException("ERROR: Must specify a bool.");
                    return SetBossHealthBarVisible(bossBarEnabled);

                case "SetShouldCloseBossHealthBar":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify both an NPC and whether or not the NPC's health bar should be closed as a bool.");
                        if (args.Length < 3)
                            return new ArgumentNullException("ERROR: Must specify whether or not the NPC's health bar should be closed as a bool.");
                        if (!(args[2] is float) && !(args[2] is bool))
                            return new ArgumentException("ERROR: The second argument to \"SetShouldCloseBossHealthBar\" must be a bool.");
                        if (!isValidNPCArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"SetShouldCloseBossHealthBar\" must be an NPC.");

                        SetShouldCloseBossHealthBar(castNPC(args[1]), (bool)args[2]);
                        return null;
                    }

                case "GetShouldCloseBossHealthbar":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify an NPC.");
                        if (!isValidNPCArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"GetShouldCloseBossHealthBar\" must be an NPC.");
                        return GetShouldCloseBossHealthBar(castNPC(args[1]));
                    }

                case "NoDodges":
                case "DodgesDisabled":
                case "GetDodgesDisabled":
                    return AreDodgesDisabled();

                case "DisableDodges":
                case "DisableAllDodges":
                case "SetDodgesDisabled":
                    if (args.Length < 2 || !(args[1] is bool disableDodges))
                        return new ArgumentNullException("ERROR: Must specify a bool.");
                    return DisableAllDodges(disableDodges);

                case "SetPlayerColdImmune":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and if the player should be immune to Death Mode cold effects as a bool.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify if a player should be immune to Death Mode cold effects as a bool.");
                    if (!(args[2] is bool))
                        return new ArgumentException("ERROR: The second argument to \"SetPlayerColdImmune\" must be a bool.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"SetPlayerColdImmune\" must be a Player or an int.");
                    bool coldImmune = (bool)args[2];
                    SetPlayerColdImmune(castPlayer(args[1]), coldImmune);
                    return null;

                case "SetPlayerHeatImmune":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and if the player should be immune to Death Mode heat effects as a bool.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify if a player should be immune to Death Mode heat effects as a bool.");
                    if (!(args[2] is bool))
                        return new ArgumentException("ERROR: The second argument to \"SetPlayerHeatImmune\" must be a bool.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"SetPlayerHeatImmune\" must be a Player or an int.");
                    bool heatImmune = (bool)args[2];
                    SetPlayerHeatImmune(castPlayer(args[1]), heatImmune);
                    return null;

                case "SetPlayerDefenseDamageImmune":
                    if (args.Length < 2)
                        return new ArgumentNullException("ERROR: Must specify both a Player object (or int index of a Player) and if the player should be immune to defense damage as a bool.");
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify if a player should be immune to defense damage as a bool.");
                    if (!(args[2] is bool))
                        return new ArgumentException("ERROR: The second argument to \"SetPlayerDefenseDamageImmune\" must be a bool.");
                    if (!isValidPlayerArg(args[1]))
                        return new ArgumentException("ERROR: The first argument to \"SetPlayerDefenseDamageImmune\" must be a Player or an int.");
                    bool defenseDamageImmune = (bool)args[2];
                    SetPlayerDefenseDamageImmune(castPlayer(args[1]), defenseDamageImmune);
                    return null;

                case "AcidRainActive":
                case "IsAcidRainActive":
                case "GetAcidRainActive":
                    return AcidRainEvent.AcidRainEventIsOngoing;

                case "StartAcidRain":
                    AcidRainEvent.TryStartEvent(true);
                    CalamityNetcode.SyncWorld();
                    return true;

                case "StopAcidRain":
                    if (AcidRainEvent.AcidRainEventIsOngoing)
                    {
                        AcidRainEvent.AccumulatedKillPoints = 0;
                        AcidRainEvent.HasTriedToSummonOldDuke = false;
                        AcidRainEvent.UpdateInvasion(false);
                    }
                    return true;

                // This is intentionally separate from the above because it will stop other events when they are added.
                case "AbominationnClearEvents":
                    bool eventActive = AcidRainEvent.AcidRainEventIsOngoing;
                    bool canClear = Convert.ToBoolean(args[1]); //This is to indicate whether abomm is able to clear the event due to a cooldown
                    if (eventActive && canClear) //adjust based on other events when added.
                    {
                        AcidRainEvent.AccumulatedKillPoints = 0;
                        AcidRainEvent.HasTriedToSummonOldDuke = false;
                        AcidRainEvent.UpdateInvasion(false);
                    }
                    return eventActive;

                case "CreateEnchantment":
                case "RegisterEnchantment":
                    EnchantmentManager.ConstructFromModcall(args.Skip(1));
                    return null;

                case "MakeItemExhumable":
                    if (args.Length != 3)
                        return new ArgumentNullException("ERROR: Must specify two Item types as an int.");
                    if (!castID(args[1], out int toExhume))
                        return new ArgumentException("ERROR: The first argument to \"MakeItemExhumable\" must be an int or short ID.");
                    if (!castID(args[2], out int result))
                        return new ArgumentException("ERROR: The second argument to \"MakeItemExhumable\" must be an int or short ID.");
                    EnchantmentManager.ItemUpgradeRelationship[toExhume] = result;
                    return null;

                case "DeclareMiniboss":
                case "DeclareMinibossForHealthBar":
                    if (args.Length != 2)
                        return new ArgumentNullException("ERROR: Must specify both an NPC type as an int.");
                    if (!castID(args[1], out int npcType))
                        return new ArgumentException("ERROR: The first argument to \"DeclareMiniboss\" must be an int or short ID.");

                    BossHealthBarManager.MinibossHPBarList.Add(npcType);
                    return null;

                case "ExcludeBossFromHealthBar":
                    if (args.Length != 2)
                        return new ArgumentNullException("ERROR: Must specify both an NPC type as an int.");
                    if (!castID(args[1], out int npcType2))
                        return new ArgumentException("ERROR: The first argument to \"ExcludeBossFromHealthBar\" must be an int or short ID.");

                    BossHealthBarManager.BossExclusionList.Add(npcType2);
                    return null;

                case "DeclareOneToManyRelationshipForHealthBar":
                    if (args.Length < 3)
                        return new ArgumentNullException("ERROR: Must specify both an NPC type as an int for the first argument and the other NPC types in the relationship as ints for the rest of the arguments.");
                    if (!args.Skip(1).All(a => castID(a, out _)))
                        return new ArgumentException("ERROR: All secondary and onward arguments to \"DeclareOneToManyRelationshipForHealthBar\" must be int or short IDs.");

                    castID(args[1], out int npcType3);

                    int[] npcsInRelationship = args.Skip(2).Select(a => (int)a).ToArray();
                    BossHealthBarManager.OneToMany[npcType3] = npcsInRelationship;
                    return null;

                // For context, the boolean argument in the second delegate refers to whether the function should be accumulating max life (true) or just life (false), and the returned long should be the accumulated health.
                case "DeclareSpecialHPCalculationDecisionForHealthBar":
                    if (args.Length != 3)
                        return new ArgumentNullException("ERROR: Must specify both a usage requirement as a Func<NPC, bool> and a health calculator function as a Func<NPC, bool, long>.");
                    if (!(args[1] is Func<NPC, bool> usageRequirement))
                        return new ArgumentException("ERROR: The first argument to \"DeclareSpecialHPCalculationDecisionForHealthBar\" must be a Func<NPC, bool>.");
                    if (!(args[2] is Func<NPC, bool, long> healthCalculatorFunction))
                        return new ArgumentException("ERROR: The first argument to \"DeclareSpecialHPCalculationDecisionForHealthBar\" must be a Func<NPC, bool, long>.");

                    BossHealthBarManager.SpecialHPRequirements[new BossHealthBarManager.NPCSpecialHPGetRequirement(usageRequirement)] = new BossHealthBarManager.NPCSpecialHPGetFunction(healthCalculatorFunction);
                    return null;

                case "CreateNameExtensionHandlerForHealthBar":
                    if (args.Length < 4)
                        return new ArgumentNullException("ERROR: Must specify a extension name as a string, the main NPC type as an int, and the other NPC types to check for as ints the rest of the arguments.");
                    if (!(args[1] is LocalizedText name))
                        return new ArgumentException("ERROR: The first argument to \"CreateNameExtensionHandlerForHealthBar\" must be a LocalizedText.");
                    if (!castID(args[1], out int npcType4))
                        return new ArgumentException("ERROR: The second argument to \"CreateNameExtensionHandlerForHealthBar\" must be an int or short ID.");
                    if (!args.Skip(3).All(a => a is int))
                        return new ArgumentException("ERROR: All ternary and onward arguments to \"CreateNameExtensionHandlerForHealthBar\" must be ints.");

                    int[] npcsToCheckFor = args.Skip(3).Select(a => (int)a).ToArray();
                    BossHealthBarManager.EntityExtensionHandler[npcType4] = new BossHealthBarManager.BossEntityExtension(name, npcsToCheckFor);
                    return null;

                // In the following two mod calls, the first argument is the NPC type, the second is the time change context (-1 being night, 0 being nothing, and 1 being day),
                // the third being the boss spawning function, the fourth being the overriding countdown to use, the fifth being whether the boss uses a special sound on spawning,
                // the sixth being the dimness factor that Boss Rush should become once the boss is currently present, the seventh being the array of NPCs present in the battle that
                // should not be deleted by the Boss Rush itself, and the eight being the potential NPCs that will end up killing, assuming the initial boss isn't
                // that (such as P1 Hive Mind turning into its second form and you being expected to kill that).
                case "GetBossRushEntries":
                    var entries = new List<(int, int, Action<int>, int, bool, float, int[], int[])>();
                    foreach (BossRushEvent.Boss boss in BossRushEvent.Bosses)
                    {
                        int[] deathEntries = BossRushEvent.BossIDsAfterDeath.ContainsKey(boss.EntityID) ? BossRushEvent.BossIDsAfterDeath[boss.EntityID] : null;
                        entries.Add((boss.EntityID, (int)boss.ToChangeTimeTo, new Action<int>(boss.SpawnContext), boss.SpecialSpawnCountdown, boss.UsesSpecialSound, boss.DimnessFactor, boss.HostileNPCsToNotDelete.ToArray(), deathEntries));
                    }

                    return entries;

                case "SetBossRushEntries":
                    if (args.Length != 2)
                        return new ArgumentNullException("ERROR: Must specify a list of bosses as a List<(int, int, Action<int>, int, bool, int[], int[])>.");
                    if (!(args[1] is List<(int, int, Action<int>, int, bool, float, int[], int[])> entries2))
                        return new ArgumentException("ERROR: The first argument to \"SetBossRushEntries\" must be a List<(int, int, Action<int>, int, bool, int[], int[])>.");

                    BossRushEvent.Bosses.Clear();
                    BossRushEvent.BossIDsAfterDeath.Clear();
                    foreach (var entry in entries2)
                    {
                        if (entry.Item8 != null)
                            BossRushEvent.BossIDsAfterDeath[entry.Item1] = entry.Item8;
                        BossRushEvent.Bosses.Add(new BossRushEvent.Boss(entry.Item1, (BossRushEvent.TimeChangeContext)entry.Item2, new BossRushEvent.Boss.OnSpawnContext(entry.Item3), entry.Item4, entry.Item5, entry.Item6, entry.Item7));
                    }

                    return null;

                case "CreateCustomDeathEffectForBossRush":
                    if (args.Length != 3)
                        return new ArgumentNullException("ERROR: Must specify both an NPC type and an Action<NPC> that determines what happens when the NPC is killed.");
                    if (!castID(args[1], out int npcType5))
                        return new ArgumentException("ERROR: The first argument to \"CreateCustomDeathEffectForBossRush\" must be an int or short ID.");
                    if (!(args[2] is Action<NPC> deathEffect))
                        return new ArgumentException("ERROR: The first argument to \"CreateCustomDeathEffectForBossRush\" must be an Action<NPC>.");

                    BossRushEvent.BossDeathEffects[npcType5] = deathEffect;
                    return null;

                case "LoadParticleInstances":
                    CalamityMod.Log.Warn("This mod call is deprecated. Calamity automatically registers particles.");
                    return null;

                case "RegisterModCooldowns":
                    CalamityMod.Log.Warn("This mod call is deprecated. Calamity automatically registers cooldowns.");
                    return null;

                case "CreateCodebreakerDialogOption":
                    // NOTE: This is a legacy variant of this call. The variant with three arguments is the standard.
                    if (args.Length == 4)
                    {
                        if (args[1] is not string inquiry || args[2] is not string response || args[3] is not Func<bool> condition)
                            throw new ArgumentException("ERROR: Must specify a string that determines the inquiry, a string that determines the response, and a Func<bool> that determines the condition for the three argument call.");
                        DraedonDialogRegistry.DialogOptions.Add(new(inquiry, response, condition));
                    }
                    else if (args.Length == 3)
                    {
                        if (args[1] is not string localizationKey || args[2] is not Func<bool> condition)
                            throw new ArgumentException("ERROR: Must specify a string that determines the localization key and a Func<bool> that determines the condition for the two argument call.");
                        DraedonDialogRegistry.DialogOptions.Add(new(localizationKey, condition));
                    }
                    else
                        throw new ArgumentException("ERROR: Must specify either two or three arguments.");

                    return null;

                case "RegisterDebuff":
                case "AddToDebuffDisplay":
                case "AddDebuffDisplay":
                case "DisplayDebuff":
                case "DebuffIcon":
                    {
                        if (args.Length < 2 || args[1] is not string texturePath)
                            return new ArgumentException("ERROR: The first argument to \"RegisterDebuff\" must be the texture path to the debuff sprite as a string");
                        if (args.Length != 3 || args[2] is not Predicate<NPC> debuffCheck)
                            return new ArgumentException("ERROR: The second argument to \"RegisterDebuff\" Must be a Predicate<NPC> that checks if an NPC meets the conditions for the debuff.");
                        RegisterDebuff(texturePath, debuffCheck);
                        return null;
                    }

                case "RegisterNPCShop":
                case "RegisterShop":
                case "RegisterTownNPCShop":
                case "AddNPCShop":
                case "AddShop":
                case "AddTownNPCShop":
                    {
                        if (args.Length < 2 || args[1] is not int npc)
                            return new ArgumentException("ERROR: The first argument to \"RegisterTownNPCShop\" must be the id of the NPC");
                        if (args.Length < 3 || args[2] is not Predicate<Player> shopCheck)
                            return new ArgumentException("ERROR: The second argument to \"RegisterTownNPCShop\" Must be a Predicate<Player> that checks if the new shop variable bool is true or not.");
                        if (args.Length != 4 || args[3] is not Action<Player, bool> shopSet)
                            return new ArgumentException("ERROR: The third argument to \"RegisterTownNPCShop\" Must be a Action<Player, bool> that is able to get and set the player's shop variable bool.");
                        RegisterTownNPCShop(npc, shopCheck, shopSet);
                        return null;
                    }

                case "SetNewShopVariable":
                case "SendNPCShopAlert":
                case "SendNPCAlert":
                    {
                        if (args.Length < 2 || args[1] is not int[] npcs)
                            return new ArgumentException("ERROR: The first argument to \"SetNewShopVariable\" must be an integer array of npc ids that should be alerted.");
                        if (args.Length != 3 || args[2] is not bool alreadySet)
                            return new ArgumentException("ERROR: The second argument to \"SetNewShopVariable\" Must be a bool that determines if the shop alert should show.");
                        CalamityGlobalTownNPC.SetNewShopVariable(npcs, alreadySet);
                        return null;
                    }

                case "BossHealthBoost":
                case "GetBossHealthBoost":
                case "BossHealthMultiplier":
                case "GetBossHealthMultiplier":
                    return CalamityServerConfig.Instance.BossHealthBoost;

                case "HasPermanentPowerup":
                case "GetPermanentPowerup":
                case "GetPowerup":
                case "HasPermanentBooster":
                case "GetPermanentBooster":
                case "GetBooster":
                    {
                        if (!isValidPlayerArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"HasPermanentPowerup\" must be a Player.");
                        if (args[2] is not string powerupName)
                            return new ArgumentException("ERROR: The second argument to \"HasPermanentPowerup\" must be a string.");

                        return HasPermanentPowerup(castPlayer(args[1]), powerupName);
                    }

                case "SetPermanentPowerup":
                case "SetPowerup":
                case "SetPermanentBooster":
                case "SetBooster":
                    {
                        if (!isValidPlayerArg(args[1]))
                            return new ArgumentException("ERROR: The first argument to \"HasPermanentPowerup\" must be a Player.");
                        if (args[2] is not string powerupName)
                            return new ArgumentException("ERROR: The second argument to \"HasPermanentPowerup\" must be a string.");
                        if (args[3] is not bool value)
                            return new ArgumentException("ERROR: The third argument to \"HasPermanentPowerup\" must be a bool.");

                        SetPermanentPowerup(castPlayer(args[1]), powerupName, value);
                        return null;
                    }

                case "RegisterAndroombaSolution":
                case "RegisterAndroombaState":
                case "RegisterAndroomba":
                case "AddAndrombaSolution":
                case "AddAndroombaState":
                case "AddAndroomba":
                    {
                        if (args[1] is not int itemID)
                            return new ArgumentException("ERROR: The first argument to \"RegisterAndroombaSolution\" must be the ID of a solution as an int.");
                        if (args[2] is not string texturePath)
                            return new ArgumentException("ERROR: The second argument to \"RegisterAndroombaSolution\" must be a string path.");
                        if (args[3] is not Action<NPC> NPCaction)
                            return new ArgumentException("ERROR: The third argument to \"RegisterAndroombaSolution\" must be an Action<NPC>.");

                        AndroombaFriendly.customConversionTypes.Add((itemID, texturePath, NPCaction));
                        return null;
                    }
                case "GetDebuffDamage":
                    {
                        if (args.Length < 2)
                            return new ArgumentNullException("ERROR: Must specify an NPC and a buff type int");
                        if (args[1] is not NPC npc || args[2] is not int type)
                            return new ArgumentException("ERROR: The first argument to \"GetDebuffDamage\" must be an NPC, and the second must be an int.");
                        return DebuffData.GetDebuffRegenValue(npc, type);
                    }
                case "GetDebuffDamageFunction": //This exists because Calamity's Mod Call lookup is slow due to this string switch, and this method may be called hundreds of times per frame by some addons.
                    return (object)DebuffData.GetDebuffRegenValue;
                default:
                    {
                        CalamityMod.Log.Warn($"{methodName} is an invalid method name for Calamity Mod. Are you calling a method which no longer exists?");
                        return null;
                    }
            }
        }
        #endregion
    }
}

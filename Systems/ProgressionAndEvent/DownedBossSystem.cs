using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

// This does not use the Systems directory in its namespace for the sake of allowing ease of access.
// It can be changed at some point in the future if really necessary but there are hundreds of references to downed bools and it just
// sounds like more trouble than it's worth.
namespace CalamityMod
{
    public class DownedBossSystem : ModSystem
    {
        private sealed class DownedFlag
        {
            public readonly string Tag;
            public readonly Func<bool> Get;
            public readonly Action<bool> Set;

            public DownedFlag(string tag, Func<bool> get, Action<bool> set)
            {
                Tag = tag;
                Get = get;
                Set = set;
            }
        }

        // Bosses
        internal static bool _downedDesertScourge = false;
        internal static bool _downedCrabulon = false;
        internal static bool _downedHiveMind = false;
        internal static bool _downedPerforator = false;
        internal static bool _downedSlimeGod = false;
        internal static bool _downedCryogen = false;
        internal static bool _downedAquaticScourge = false;
        internal static bool _downedBrimstoneElemental = false;
        internal static bool _downedCalamitasClone = false;
        internal static bool _downedLeviathan = false;
        internal static bool _downedAstrumAureus = false;
        internal static bool _downedPlaguebringer = false;
        internal static bool _downedRavager = false;
        internal static bool _downedAstrumDeus = false;
        internal static bool _downedGuardians = false;
        internal static bool _downedDragonfolly = false;
        internal static bool _downedProvidence = false;
        internal static bool _downedCeaselessVoid = false;
        internal static bool _downedStormWeaver = false;
        internal static bool _downedSignus = false;
        internal static bool _downedSecondSentinels = false; // TODO: UNUSED (DoG no longer has sentinels phase)
        internal static bool _downedPolterghast = false;
        internal static bool _downedBoomerDuke = false;
        internal static bool _downedDoG = false;
        internal static bool _downedYharon = false;
        internal static bool _downedAres = false; // only used for loot drops
        internal static bool _downedThanatos = false; // only used for loot drops
        internal static bool _downedArtemisAndApollo = false; // only used for loot drops
        internal static bool _downedExoMechs = false;
        internal static bool _downedCalamitas = false;
        internal static bool _downedPrimordialWyrm = false;

        // Minibosses
        internal static bool _downedGSS = false;
        internal static bool _downedHorribleHog = false;
        internal static bool _downedCLAM = false;
        internal static bool _downedCLAMHardMode = false;
        internal static bool _downedCragmawMire = false;
        internal static bool _downedMauler = false;
        internal static bool _downedNuclearTerror = false;

        // Events
        internal static bool _downedEoCAcidRain = false;
        internal static bool _downedAquaticScourgeAcidRain = false;
        internal static bool _startedBossRushAtLeastOnce = false;
        internal static bool _downedBossRush = false;

        // Betsy and Dreadnautilus because vanilla doesn't track them
        internal static bool _downedBetsy = false;
        internal static bool _downedDreadnautilus = false;

        // Keep this table in LoadWorldData assignment order. SaveOrder and ResetOrder refer to
        // these zero-based positions to preserve their existing order independently.
        private static readonly DownedFlag[] DownedFlags =
        {
            new DownedFlag("desertScourge", () => downedDesertScourge, value => downedDesertScourge = value),
            new DownedFlag("aquaticScourge", () => downedAquaticScourge, value => downedAquaticScourge = value),
            new DownedFlag("crabulon", () => downedCrabulon, value => downedCrabulon = value),
            new DownedFlag("hiveMind", () => downedHiveMind, value => downedHiveMind = value),
            new DownedFlag("perforator", () => downedPerforator, value => downedPerforator = value),
            new DownedFlag("slimeGod", () => downedSlimeGod, value => downedSlimeGod = value),
            new DownedFlag("dreadnautilus", () => downedDreadnautilus, value => downedDreadnautilus = value),
            new DownedFlag("cryogen", () => downedCryogen, value => downedCryogen = value),
            new DownedFlag("brimstoneElemental", () => downedBrimstoneElemental, value => downedBrimstoneElemental = value),
            new DownedFlag("calamitas", () => downedCalamitasClone, value => downedCalamitasClone = value),
            new DownedFlag("leviathan", () => downedLeviathan, value => downedLeviathan = value),
            new DownedFlag("astrageldon", () => downedAstrumAureus, value => downedAstrumAureus = value),
            new DownedFlag("betsy", () => downedBetsy, value => downedBetsy = value),
            new DownedFlag("plaguebringerGoliath", () => downedPlaguebringer, value => downedPlaguebringer = value),
            new DownedFlag("scavenger", () => downedRavager, value => downedRavager = value),
            new DownedFlag("starGod", () => downedAstrumDeus, value => downedAstrumDeus = value),
            new DownedFlag("guardians", () => downedGuardians, value => downedGuardians = value),
            new DownedFlag("bumblebirb", () => downedDragonfolly, value => downedDragonfolly = value),
            new DownedFlag("providence", () => downedProvidence, value => downedProvidence = value),
            new DownedFlag("ceaselessVoid", () => downedCeaselessVoid, value => downedCeaselessVoid = value),
            new DownedFlag("stormWeaver", () => downedStormWeaver, value => downedStormWeaver = value),
            new DownedFlag("signus", () => downedSignus, value => downedSignus = value),
            new DownedFlag("polterghast", () => downedPolterghast, value => downedPolterghast = value),
            new DownedFlag("oldDuke", () => downedBoomerDuke, value => downedBoomerDuke = value),
            new DownedFlag("secondSentinels", () => downedSecondSentinels, value => downedSecondSentinels = value),
            new DownedFlag("devourerOfGods", () => downedDoG, value => downedDoG = value),
            new DownedFlag("yharon", () => downedYharon, value => downedYharon = value),
            new DownedFlag("thanatos", () => downedThanatos, value => downedThanatos = value),
            new DownedFlag("artemisAndApollo", () => downedArtemisAndApollo, value => downedArtemisAndApollo = value),
            new DownedFlag("ares", () => downedAres, value => downedAres = value),
            new DownedFlag("exoMechs", () => downedExoMechs, value => downedExoMechs = value),
            new DownedFlag("supremeCalamitas", () => downedCalamitas, value => downedCalamitas = value),
            new DownedFlag("adultEidolonWyrm", () => downedPrimordialWyrm, value => downedPrimordialWyrm = value),
            new DownedFlag("horribleHog", () => downedHorribleHog, value => downedHorribleHog = value),
            new DownedFlag("clam", () => downedCLAM, value => downedCLAM = value),
            new DownedFlag("eocRain", () => downedEoCAcidRain, value => downedEoCAcidRain = value),
            new DownedFlag("clamHardmode", () => downedCLAMHardMode, value => downedCLAMHardMode = value),
            new DownedFlag("cragmawMire", () => downedCragmawMire, value => downedCragmawMire = value),
            new DownedFlag("hmRain", () => downedAquaticScourgeAcidRain, value => downedAquaticScourgeAcidRain = value),
            new DownedFlag("greatSandShark", () => downedGSS, value => downedGSS = value),
            new DownedFlag("mauler", () => downedMauler, value => downedMauler = value),
            new DownedFlag("nuclearTerror", () => downedNuclearTerror, value => downedNuclearTerror = value),
            new DownedFlag("startedBossRush", () => startedBossRushAtLeastOnce, value => startedBossRushAtLeastOnce = value),
            new DownedFlag("bossRush", () => downedBossRush, value => downedBossRush = value)
        };

        private static readonly int[] SaveOrder =
        {
            // Pre-hardmode bosses
            0, 2, 3, 4, 5,
            // Hardmode bosses
            6, 7, 1, 8, 9, 10, 11, 12, 13, 14, 15,
            // Post-ML bosses
            16, 17, 18, 19, 20, 21, 24, 22, 23, 25, 26, 27, 28, 29, 30, 31, 32,
            // Minibosses and events
            33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43
        };

        private static readonly int[] ResetOrder =
        {
            0, 2, 3, 4, 5, 6, 7, 1, 8, 9, 10, 11, 12, 13, 14, 15,
            16, 17, 18, 19, 20, 21, 22, 23, 25, 26, 29, 27, 28, 30, 31, 32, 24,
            33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43
        };

        #region Wrapper Properties for Lantern Nights
        public static bool downedDesertScourge { get => _downedDesertScourge; set => SetLanternNightFlag(ref _downedDesertScourge, value); }
        public static bool downedCrabulon { get => _downedCrabulon; set => SetLanternNightFlag(ref _downedCrabulon, value); }
        public static bool downedHiveMind { get => _downedHiveMind; set => SetLanternNightFlag(ref _downedHiveMind, value); }
        public static bool downedPerforator { get => _downedPerforator; set => SetLanternNightFlag(ref _downedPerforator, value); }
        public static bool downedSlimeGod { get => _downedSlimeGod; set => SetLanternNightFlag(ref _downedSlimeGod, value); }
        public static bool downedDreadnautilus { get => _downedDreadnautilus; set => SetLanternNightFlag(ref _downedDreadnautilus, value); }
        public static bool downedCryogen { get => _downedCryogen; set => SetLanternNightFlag(ref _downedCryogen, value); }
        public static bool downedAquaticScourge { get => _downedAquaticScourge; set => SetLanternNightFlag(ref _downedAquaticScourge, value); }
        public static bool downedBrimstoneElemental { get => _downedBrimstoneElemental; set => SetLanternNightFlag(ref _downedBrimstoneElemental, value); }
        public static bool downedCalamitasClone { get => _downedCalamitasClone; set => SetLanternNightFlag(ref _downedCalamitasClone, value); }
        public static bool downedLeviathan { get => _downedLeviathan; set => SetLanternNightFlag(ref _downedLeviathan, value); }
        public static bool downedAstrumAureus { get => _downedAstrumAureus; set => SetLanternNightFlag(ref _downedAstrumAureus, value); }
        public static bool downedBetsy { get => _downedBetsy; set => SetLanternNightFlag(ref _downedBetsy, value); }
        public static bool downedPlaguebringer { get => _downedPlaguebringer; set => SetLanternNightFlag(ref _downedPlaguebringer, value); }
        public static bool downedRavager { get => _downedRavager; set => SetLanternNightFlag(ref _downedRavager, value); }
        public static bool downedAstrumDeus { get => _downedAstrumDeus; set => SetLanternNightFlag(ref _downedAstrumDeus, value); }
        public static bool downedGuardians { get => _downedGuardians; set => SetLanternNightFlag(ref _downedGuardians, value); }
        public static bool downedDragonfolly { get => _downedDragonfolly; set => SetLanternNightFlag(ref _downedDragonfolly, value); }
        public static bool downedProvidence { get => _downedProvidence; set => SetLanternNightFlag(ref _downedProvidence, value); }
        public static bool downedCeaselessVoid { get => _downedCeaselessVoid; set => SetLanternNightFlag(ref _downedCeaselessVoid, value); }
        public static bool downedStormWeaver { get => _downedStormWeaver; set => SetLanternNightFlag(ref _downedStormWeaver, value); }
        public static bool downedSignus { get => _downedSignus; set => SetLanternNightFlag(ref _downedSignus, value); }
        public static bool downedPolterghast { get => _downedPolterghast; set => SetLanternNightFlag(ref _downedPolterghast, value); }
        public static bool downedBoomerDuke { get => _downedBoomerDuke; set => SetLanternNightFlag(ref _downedBoomerDuke, value); }
        public static bool downedDoG { get => _downedDoG; set => SetLanternNightFlag(ref _downedDoG, value); }
        public static bool downedYharon { get => _downedYharon; set => SetLanternNightFlag(ref _downedYharon, value); }
        public static bool downedExoMechs { get => _downedExoMechs; set => SetLanternNightFlag(ref _downedExoMechs, value); }
        public static bool downedCalamitas { get => _downedCalamitas; set => SetLanternNightFlag(ref _downedCalamitas, value); }
        public static bool downedPrimordialWyrm { get => _downedPrimordialWyrm; set => SetLanternNightFlag(ref _downedPrimordialWyrm, value); }
        public static bool downedHorribleHog { get => _downedHorribleHog; set => SetLanternNightFlag(ref _downedHorribleHog, value); }
        public static bool downedCLAM { get => _downedCLAM; set => SetLanternNightFlag(ref _downedCLAM, value); }
        public static bool downedCLAMHardMode { get => _downedCLAMHardMode; set => SetLanternNightFlag(ref _downedCLAMHardMode, value); }
        public static bool downedCragmawMire { get => _downedCragmawMire; set => SetLanternNightFlag(ref _downedCragmawMire, value); }
        public static bool downedGSS { get => _downedGSS; set => SetLanternNightFlag(ref _downedGSS, value); }
        public static bool downedMauler { get => _downedMauler; set => SetLanternNightFlag(ref _downedMauler, value); }
        public static bool downedNuclearTerror { get => _downedNuclearTerror; set => SetLanternNightFlag(ref _downedNuclearTerror, value); }
        private static void SetLanternNightFlag(ref bool flag, bool value)
        {
            if (!value)
                flag = false;
            else
                NPC.SetEventFlagCleared(ref flag, -1);
        }

        #endregion

        // These are purely used for loot drops, nothing else
        public static bool downedSecondSentinels
        {
            get => _downedSecondSentinels;
            set => _downedSecondSentinels = value;
        }

        public static bool downedAres
        {
            get => _downedAres;
            set => _downedAres = value;
        }
        public static bool downedThanatos
        {
            get => _downedThanatos;
            set => _downedThanatos = value;
        }
        public static bool downedArtemisAndApollo
        {
            get => _downedArtemisAndApollo;
            set => _downedArtemisAndApollo = value;
        }

        public static bool downedEoCAcidRain { get => _downedEoCAcidRain; set => SetLanternNightFlag(ref _downedEoCAcidRain, value); }
        public static bool downedAquaticScourgeAcidRain { get => _downedAquaticScourgeAcidRain; set => SetLanternNightFlag(ref _downedAquaticScourgeAcidRain, value); }
        public static bool startedBossRushAtLeastOnce
        {
            get => _startedBossRushAtLeastOnce;
            set => _startedBossRushAtLeastOnce = value; // Starting Boss Rush for the first time does not trigger a Lantern Night
        }
        public static bool downedBossRush { get => _downedBossRush; set => SetLanternNightFlag(ref _downedBossRush, value); }

        internal static void ResetAllFlags()
        {
            foreach (int flagIndex in ResetOrder)
                DownedFlags[flagIndex].Set(false);
        }

        //public override void OnWorldLoad() => ResetAllFlags();

        //public override void OnWorldUnload() => ResetAllFlags();

        public override void SaveWorldData(TagCompound tag)
        {
            List<string> downed = new List<string>();
            foreach (int flagIndex in SaveOrder)
            {
                DownedFlag flag = DownedFlags[flagIndex];
                if (flag.Get())
                    downed.Add(flag.Tag);
            }

            tag["downedFlags"] = downed;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("downedFlags");
            foreach (DownedFlag flag in DownedFlags)
                flag.Set(downed.Contains(flag.Tag));
        }
    }
}

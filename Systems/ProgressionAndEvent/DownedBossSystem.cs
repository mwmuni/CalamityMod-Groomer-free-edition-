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

        #region Wrapper Properties for Lantern Nights
        public static bool downedDesertScourge
        {
            get => _downedDesertScourge;
            set => SetLanternNightFlag(ref _downedDesertScourge, value);
        }
        public static bool downedCrabulon
        {
            get => _downedCrabulon;
            set => SetLanternNightFlag(ref _downedCrabulon, value);
        }
        public static bool downedHiveMind
        {
            get => _downedHiveMind;
            set => SetLanternNightFlag(ref _downedHiveMind, value);
        }
        public static bool downedPerforator
        {
            get => _downedPerforator;
            set => SetLanternNightFlag(ref _downedPerforator, value);
        }
        public static bool downedSlimeGod
        {
            get => _downedSlimeGod;
            set => SetLanternNightFlag(ref _downedSlimeGod, value);
        }
        public static bool downedDreadnautilus
        {
            get => _downedDreadnautilus;
            set => SetLanternNightFlag(ref _downedDreadnautilus, value);
        }
        public static bool downedCryogen
        {
            get => _downedCryogen;
            set => SetLanternNightFlag(ref _downedCryogen, value);
        }
        public static bool downedAquaticScourge
        {
            get => _downedAquaticScourge;
            set => SetLanternNightFlag(ref _downedAquaticScourge, value);
        }
        public static bool downedBrimstoneElemental
        {
            get => _downedBrimstoneElemental;
            set => SetLanternNightFlag(ref _downedBrimstoneElemental, value);
        }
        public static bool downedCalamitasClone
        {
            get => _downedCalamitasClone;
            set => SetLanternNightFlag(ref _downedCalamitasClone, value);
        }
        public static bool downedLeviathan
        {
            get => _downedLeviathan;
            set => SetLanternNightFlag(ref _downedLeviathan, value);
        }
        public static bool downedAstrumAureus
        {
            get => _downedAstrumAureus;
            set => SetLanternNightFlag(ref _downedAstrumAureus, value);
        }
        public static bool downedBetsy
        {
            get => _downedBetsy;
            set => SetLanternNightFlag(ref _downedBetsy, value);
        }
        public static bool downedPlaguebringer
        {
            get => _downedPlaguebringer;
            set => SetLanternNightFlag(ref _downedPlaguebringer, value);
        }
        public static bool downedRavager
        {
            get => _downedRavager;
            set => SetLanternNightFlag(ref _downedRavager, value);
        }
        public static bool downedAstrumDeus
        {
            get => _downedAstrumDeus;
            set => SetLanternNightFlag(ref _downedAstrumDeus, value);
        }
        public static bool downedGuardians
        {
            get => _downedGuardians;
            set => SetLanternNightFlag(ref _downedGuardians, value);
        }
        public static bool downedDragonfolly
        {
            get => _downedDragonfolly;
            set => SetLanternNightFlag(ref _downedDragonfolly, value);
        }
        public static bool downedProvidence
        {
            get => _downedProvidence;
            set => SetLanternNightFlag(ref _downedProvidence, value);
        }
        public static bool downedCeaselessVoid
        {
            get => _downedCeaselessVoid;
            set => SetLanternNightFlag(ref _downedCeaselessVoid, value);
        }
        public static bool downedStormWeaver
        {
            get => _downedStormWeaver;
            set => SetLanternNightFlag(ref _downedStormWeaver, value);
        }
        public static bool downedSignus
        {
            get => _downedSignus;
            set => SetLanternNightFlag(ref _downedSignus, value);
        }
        public static bool downedPolterghast
        {
            get => _downedPolterghast;
            set => SetLanternNightFlag(ref _downedPolterghast, value);
        }
        public static bool downedBoomerDuke
        {
            get => _downedBoomerDuke;
            set => SetLanternNightFlag(ref _downedBoomerDuke, value);
        }
        public static bool downedDoG
        {
            get => _downedDoG;
            set => SetLanternNightFlag(ref _downedDoG, value);
        }
        public static bool downedYharon
        {
            get => _downedYharon;
            set => SetLanternNightFlag(ref _downedYharon, value);
        }
        public static bool downedExoMechs
        {
            get => _downedExoMechs;
            set => SetLanternNightFlag(ref _downedExoMechs, value);
        }
        public static bool downedCalamitas
        {
            get => _downedCalamitas;
            set => SetLanternNightFlag(ref _downedCalamitas, value);
        }
        public static bool downedPrimordialWyrm
        {
            get => _downedPrimordialWyrm;
            set => SetLanternNightFlag(ref _downedPrimordialWyrm, value);
        }
        public static bool downedHorribleHog
        {
            get => _downedHorribleHog;
            set => SetLanternNightFlag(ref _downedHorribleHog, value);
        }
        public static bool downedCLAM
        {
            get => _downedCLAM;
            set => SetLanternNightFlag(ref _downedCLAM, value);
        }
        public static bool downedCLAMHardMode
        {
            get => _downedCLAMHardMode;
            set => SetLanternNightFlag(ref _downedCLAMHardMode, value);
        }
        public static bool downedCragmawMire
        {
            get => _downedCragmawMire;
            set => SetLanternNightFlag(ref _downedCragmawMire, value);
        }
        public static bool downedGSS
        {
            get => _downedGSS;
            set => SetLanternNightFlag(ref _downedGSS, value);
        }
        public static bool downedMauler
        {
            get => _downedMauler;
            set => SetLanternNightFlag(ref _downedMauler, value);
        }
        public static bool downedNuclearTerror
        {
            get => _downedNuclearTerror;
            set => SetLanternNightFlag(ref _downedNuclearTerror, value);
        }
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

        public static bool downedEoCAcidRain
        {
            get => _downedEoCAcidRain;
            set => SetLanternNightFlag(ref _downedEoCAcidRain, value);
        }
        public static bool downedAquaticScourgeAcidRain
        {
            get => _downedAquaticScourgeAcidRain;
            set => SetLanternNightFlag(ref _downedAquaticScourgeAcidRain, value);
        }
        public static bool startedBossRushAtLeastOnce
        {
            get => _startedBossRushAtLeastOnce;
            set => _startedBossRushAtLeastOnce = value; // Starting Boss Rush for the first time does not trigger a Lantern Night
        }
        public static bool downedBossRush
        {
            get => _downedBossRush;
            set => SetLanternNightFlag(ref _downedBossRush, value);
        }

        internal static void ResetAllFlags()
        {
            downedDesertScourge = false;
            downedCrabulon = false;
            downedHiveMind = false;
            downedPerforator = false;
            downedSlimeGod = false;
            downedDreadnautilus = false;
            downedCryogen = false;
            downedAquaticScourge = false;
            downedBrimstoneElemental = false;
            downedCalamitasClone = false;
            downedLeviathan = false;
            downedAstrumAureus = false;
            downedBetsy = false;
            downedPlaguebringer = false;
            downedRavager = false;
            downedAstrumDeus = false;
            downedGuardians = false;
            downedDragonfolly = false;
            downedProvidence = false;
            downedCeaselessVoid = false;
            downedStormWeaver = false;
            downedSignus = false;
            downedPolterghast = false;
            downedBoomerDuke = false;
            downedDoG = false;
            downedYharon = false;
            downedAres = false;
            downedThanatos = false;
            downedArtemisAndApollo = false;
            downedExoMechs = false;
            downedCalamitas = false;
            downedPrimordialWyrm = false;

            downedSecondSentinels = false;

            downedHorribleHog = false;
            downedCLAM = false;
            downedEoCAcidRain = false;

            downedCLAMHardMode = false;
            downedCragmawMire = false;
            downedAquaticScourgeAcidRain = false;
            downedGSS = false;

            downedMauler = false;
            downedNuclearTerror = false;

            startedBossRushAtLeastOnce = false;
            downedBossRush = false;
        }

        //public override void OnWorldLoad() => ResetAllFlags();

        //public override void OnWorldUnload() => ResetAllFlags();

        public override void SaveWorldData(TagCompound tag)
        {
            List<string> downed = new List<string>();

            // Pre-hardmode bosses (progression sorted)
            if (downedDesertScourge)
                downed.Add("desertScourge");
            if (downedCrabulon)
                downed.Add("crabulon");
            if (downedHiveMind)
                downed.Add("hiveMind");
            if (downedPerforator)
                downed.Add("perforator");
            if (downedSlimeGod)
                downed.Add("slimeGod");

            // Hardmode bosses (progression sorted)
            if (downedDreadnautilus)
                downed.Add("dreadnautilus");
            if (downedCryogen)
                downed.Add("cryogen");
            if (downedAquaticScourge)
                downed.Add("aquaticScourge");
            if (downedBrimstoneElemental)
                downed.Add("brimstoneElemental");
            if (downedCalamitasClone)
                downed.Add("calamitas");
            if (downedLeviathan)
                downed.Add("leviathan");
            if (downedAstrumAureus)
                downed.Add("astrageldon");
            if (downedBetsy)
                downed.Add("betsy");
            if (downedPlaguebringer)
                downed.Add("plaguebringerGoliath");
            if (downedRavager)
                downed.Add("scavenger");
            if (downedAstrumDeus)
                downed.Add("starGod");

            // Post-ML bosses (progression sorted)
            if (downedGuardians)
                downed.Add("guardians");
            if (downedDragonfolly)
                downed.Add("bumblebirb");
            if (downedProvidence)
                downed.Add("providence");
            if (downedCeaselessVoid)
                downed.Add("ceaselessVoid");
            if (downedStormWeaver)
                downed.Add("stormWeaver");
            if (downedSignus)
                downed.Add("signus");
            if (downedSecondSentinels)
                downed.Add("secondSentinels");
            if (downedPolterghast)
                downed.Add("polterghast");
            if (downedBoomerDuke)
                downed.Add("oldDuke");
            if (downedDoG)
                downed.Add("devourerOfGods");
            if (downedYharon)
                downed.Add("yharon");
            if (downedThanatos)
                downed.Add("thanatos");
            if (downedArtemisAndApollo)
                downed.Add("artemisAndApollo");
            if (downedAres)
                downed.Add("ares");
            if (downedExoMechs)
                downed.Add("exoMechs");
            if (downedCalamitas)
                downed.Add("supremeCalamitas");
            if (downedPrimordialWyrm)
                downed.Add("adultEidolonWyrm");

            // Minibosses and events
            if (downedHorribleHog)
                downed.Add("horribleHog");
            if (downedCLAM)
                downed.Add("clam");
            if (downedEoCAcidRain)
                downed.Add("eocRain");

            if (downedCLAMHardMode)
                downed.Add("clamHardmode");
            if (downedCragmawMire)
                downed.Add("cragmawMire");
            if (downedAquaticScourgeAcidRain)
                downed.Add("hmRain");
            if (downedGSS)
                downed.Add("greatSandShark");

            if (downedMauler)
                downed.Add("mauler");
            if (downedNuclearTerror)
                downed.Add("nuclearTerror");

            if (startedBossRushAtLeastOnce)
                downed.Add("startedBossRush");
            if (downedBossRush)
                downed.Add("bossRush");

            tag["downedFlags"] = downed;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("downedFlags");

            downedDesertScourge = downed.Contains("desertScourge");
            downedAquaticScourge = downed.Contains("aquaticScourge");
            downedCrabulon = downed.Contains("crabulon");
            downedHiveMind = downed.Contains("hiveMind");
            downedPerforator = downed.Contains("perforator");
            downedSlimeGod = downed.Contains("slimeGod");

            downedDreadnautilus = downed.Contains("dreadnautilus");
            downedCryogen = downed.Contains("cryogen");
            downedBrimstoneElemental = downed.Contains("brimstoneElemental");
            downedCalamitasClone = downed.Contains("calamitas");
            downedLeviathan = downed.Contains("leviathan");
            downedAstrumAureus = downed.Contains("astrageldon");
            downedBetsy = downed.Contains("betsy");
            downedPlaguebringer = downed.Contains("plaguebringerGoliath");
            downedRavager = downed.Contains("scavenger");
            downedAstrumDeus = downed.Contains("starGod");

            downedGuardians = downed.Contains("guardians");
            downedDragonfolly = downed.Contains("bumblebirb");
            downedProvidence = downed.Contains("providence");
            downedCeaselessVoid = downed.Contains("ceaselessVoid");
            downedStormWeaver = downed.Contains("stormWeaver");
            downedSignus = downed.Contains("signus");
            downedPolterghast = downed.Contains("polterghast");
            downedBoomerDuke = downed.Contains("oldDuke");
            downedSecondSentinels = downed.Contains("secondSentinels");
            downedDoG = downed.Contains("devourerOfGods");
            downedYharon = downed.Contains("yharon");
            downedThanatos = downed.Contains("thanatos");
            downedArtemisAndApollo = downed.Contains("artemisAndApollo");
            downedAres = downed.Contains("ares");
            downedExoMechs = downed.Contains("exoMechs");
            downedCalamitas = downed.Contains("supremeCalamitas");
            downedPrimordialWyrm = downed.Contains("adultEidolonWyrm");

            downedHorribleHog = downed.Contains("horribleHog");
            downedCLAM = downed.Contains("clam");
            downedEoCAcidRain = downed.Contains("eocRain");

            downedCLAMHardMode = downed.Contains("clamHardmode");
            downedCragmawMire = downed.Contains("cragmawMire");
            downedAquaticScourgeAcidRain = downed.Contains("hmRain");
            downedGSS = downed.Contains("greatSandShark");

            downedMauler = downed.Contains("mauler");
            downedNuclearTerror = downed.Contains("nuclearTerror");

            startedBossRushAtLeastOnce = downed.Contains("startedBossRush");
            downedBossRush = downed.Contains("bossRush");
        }
    }
}

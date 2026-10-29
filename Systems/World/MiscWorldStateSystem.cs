using System.Collections.Generic;
using System.IO;
using CalamityMod.CustomRecipes;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static CalamityMod.DownedBossSystem;
using static CalamityMod.World.CalamityWorld;

namespace CalamityMod
{
    public class MiscWorldStateSystem : ModSystem
    {
        #region Clear World Data (Reset)
        public override void ClearWorld()
        {
            CalamityGlobalNPC.holyBoss = -1;
            CalamityGlobalNPC.doughnutBoss = -1;
            CalamityGlobalNPC.doughnutBossDefender = -1;
            CalamityGlobalNPC.doughnutBossHealer = -1;
            CalamityGlobalNPC.voidBoss = -1;
            CalamityGlobalNPC.energyFlame = -1;
            CalamityGlobalNPC.hiveMind = -1;
            CalamityGlobalNPC.astrumAureus = -1;
            CalamityGlobalNPC.scavenger = -1;

            for (int i = 0; i < CalamityGlobalNPC.bobbitWormBottom.Length; i++)
                CalamityGlobalNPC.bobbitWormBottom[i] = -1;

            CalamityGlobalNPC.DoGHead = -1;
            CalamityGlobalNPC.SCal = -1;
            CalamityGlobalNPC.ghostBoss = -1;
            CalamityGlobalNPC.laserEye = -1;
            CalamityGlobalNPC.fireEye = -1;
            CalamityGlobalNPC.brimstoneElemental = -1;
            CalamityGlobalNPC.signus = -1;
            CalamityGlobalNPC.draedonExoMechPrimePlasmaCannon = -1;
            CalamityGlobalNPC.draedonExoMechPrime = -1;
            CalamityGlobalNPC.draedonExoMechTwinGreen = -1;
            CalamityGlobalNPC.draedonExoMechTwinRed = -1;
            CalamityGlobalNPC.draedonExoMechWorm = -1;
            CalamityGlobalNPC.adultEidolonWyrmHead = -1;
            BossRushEvent.BossRushStage = 0;
            BossRushEvent.BossRushActive = false;
            BossRushEvent.BossRushSpawnCountdown = 180;
            BossRushEvent.HostileProjectileKillCounter = 0;
            CustomTemple.NewAlterPosition = Point.Zero;
            Abyss.AbyssChasmBottom = 0;
            SulphurousSea.YStart = 0;
            Abyss.AtLeftSideOfWorld = false;
            //Abyss.AbleToUnlockChests = false; //Yet another fucking failsave for abyss chests

            spawnedBandit = false;
            foundHomePermafrost = false;
            unlockedTownPig = false;

            catName = false;
            dogName = false;
            bunnyName = false;

            revenge = false;
            TalkedToDraedon = false;
            death = false;
            armageddon = false;
            AcidRainEvent.AcidRainEventIsOngoing = false;
            AcidRainEvent.CountdownUntilForcedAcidRain = 0;

            HasGeneratedLuminitePlanetoids = false; //Allow generating planetoids in other worlds
            DownedBossSystem.ResetAllFlags();
        }
        #endregion

        #region Saving/Loading
        public override void SaveWorldData(TagCompound tag)
        {
            var downed = new List<string>();
            if (TalkedToDraedon)
                downed.Add("TalkedToDraedon");
            if (revenge)
                downed.Add("revenge");
            if (death)
                downed.Add("death");
            if (Abyss.AtLeftSideOfWorld)
                downed.Add("abyssSide");
            if (BossRushEvent.BossRushActive)
                downed.Add("bossRushActive");
            if (AcidRainEvent.AcidRainEventIsOngoing)
                downed.Add("acidRain");
            if (spawnedBandit)
                downed.Add("bandit");
            if (foundHomePermafrost)
                downed.Add("archmageHome");
            if (unlockedTownPig)
                downed.Add("townPig");

            #region Save Pet Names
            if (catName)
                downed.Add("catName");
            if (dogName)
                downed.Add("dogName");
            if (bunnyName)
                downed.Add("bunnyName");
            #endregion

            if (AcidRainEvent.HasTriedToSummonOldDuke)
                downed.Add("spawnedBoomer");
            if (AcidRainEvent.HasStartedAcidicDownpour)
                downed.Add("startDownpour");
            if (AcidRainEvent.HasBeenForceStartedByEoCDefeat)
                downed.Add("forcedRain");
            if (AcidRainEvent.OldDukeHasBeenEncountered)
                downed.Add("encounteredOldDuke");
            if (HasGeneratedLuminitePlanetoids)
                downed.Add("HasGeneratedLuminitePlanetoids");
            downed.AddWithCondition("IsWorldAfterDraedonUpdate", IsWorldAfterDraedonUpdate);

            RecipeUnlockHandler.Save(downed);

            tag["downed"] = downed;
            tag["abyssChasmBottom"] = Abyss.AbyssChasmBottom;
            tag["SulphSeaYStart"] = SulphurousSea.YStart;
            tag["AstralYStart"] = AstralBiome.YStart;
            tag["acidRainPoints"] = AcidRainEvent.AccumulatedKillPoints;
            tag["Reforges"] = Reforges;
            tag["MoneyStolenByBandit"] = MoneyStolenByBandit;

            tag["SunkenSeaLabCenter"] = SunkenSeaLabCenter;
            tag["PlanetoidLabCenter"] = PlanetoidLabCenter;
            tag["JungleLabCenter"] = JungleLabCenter;
            tag["HellLabCenter"] = HellLabCenter;
            tag["IceLabCenter"] = IceLabCenter;
            tag["CavernLabCenter"] = CavernLabCenter;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            var downed = tag.GetList<string>("downed");
            TalkedToDraedon = downed.Contains("TalkedToDraedon");
            revenge = downed.Contains("revenge");
            death = downed.Contains("death");
            Abyss.AtLeftSideOfWorld = downed.Contains("abyssSide");
            BossRushEvent.BossRushActive = downed.Contains("bossRushActive");
            AcidRainEvent.AcidRainEventIsOngoing = downed.Contains("acidRain");
            spawnedBandit = downed.Contains("bandit");
            foundHomePermafrost = downed.Contains("archmageHome");
            unlockedTownPig = downed.Contains("townPig");

            #region Load Pet Names
            catName = downed.Contains("catName");
            dogName = downed.Contains("dogName");
            bunnyName = downed.Contains("bunnyName");
            #endregion

            AcidRainEvent.HasTriedToSummonOldDuke = downed.Contains("spawnedBoomer");
            AcidRainEvent.HasStartedAcidicDownpour = downed.Contains("startDownpour");
            AcidRainEvent.HasBeenForceStartedByEoCDefeat = downed.Contains("forcedRain");
            AcidRainEvent.OldDukeHasBeenEncountered = downed.Contains("encounteredOldDuke");
            HasGeneratedLuminitePlanetoids = downed.Contains("HasGeneratedLuminitePlanetoids");
            IsWorldAfterDraedonUpdate = downed.Contains("IsWorldAfterDraedonUpdate");

            RecipeUnlockHandler.Load(downed);

            Abyss.AbyssChasmBottom = tag.GetInt("abyssChasmBottom");
            SulphurousSea.YStart = tag.GetInt("SulphSeaYStart");
            AstralBiome.YStart = tag.GetInt("AstralYStart");
            AcidRainEvent.AccumulatedKillPoints = tag.GetInt("acidRainPoints");
            Reforges = tag.GetInt("Reforges");
            MoneyStolenByBandit = tag.GetInt("MoneyStolenByBandit");

            SunkenSeaLabCenter = tag.Get<Vector2>("SunkenSeaLabCenter");
            PlanetoidLabCenter = tag.Get<Vector2>("PlanetoidLabCenter");
            JungleLabCenter = tag.Get<Vector2>("JungleLabCenter");
            HellLabCenter = tag.Get<Vector2>("HellLabCenter");
            IceLabCenter = tag.Get<Vector2>("IceLabCenter");
            CavernLabCenter = tag.Get<Vector2>("CavernLabCenter");
        }
        #endregion Saving/Loading

        #region NetSend
        public override void NetSend(BinaryWriter writer)
        {
            BitsByte flags = CreateFlags(
                downedDesertScourge, downedHiveMind, downedPerforator, downedSlimeGod,
                downedCryogen, downedBrimstoneElemental, downedCalamitasClone, downedLeviathan);
            BitsByte flags2 = CreateFlags(
                downedDoG, downedPlaguebringer, downedGuardians, downedProvidence,
                downedCeaselessVoid, downedStormWeaver, downedSignus, downedYharon);

            // Don't write meaningful values for the now-unused vanilla boss booleans
            BitsByte flags3 = CreateFlags(
                downedCalamitas, downedDragonfolly, downedCrabulon, downedBetsy,
                downedRavager, false, false, false);
            BitsByte flags4 = CreateFlags(
                false, false, false, false, false, false, false, revenge);
            BitsByte flags5 = CreateFlags(
                downedAstrumDeus, spawnedBandit, false, AcidRainEvent.HasStartedAcidicDownpour,
                false, downedPolterghast, death, downedGSS);
            BitsByte flags6 = CreateFlags(
                Abyss.AtLeftSideOfWorld, downedAquaticScourge, downedAstrumAureus, false,
                armageddon, false, false, false);
            BitsByte flags7 = CreateFlags(
                BossRushEvent.BossRushActive, downedBoomerDuke, downedCLAM, false,
                AcidRainEvent.AcidRainEventIsOngoing, downedEoCAcidRain, downedAquaticScourgeAcidRain,
                AcidRainEvent.HasTriedToSummonOldDuke);
            BitsByte flags8 = CreateFlags(
                AcidRainEvent.HasBeenForceStartedByEoCDefeat, false, downedSecondSentinels,
                foundHomePermafrost, downedCLAMHardMode, catName, dogName, bunnyName);
            BitsByte flags9 = new BitsByte();
            BitsByte flags10 = CreateFlags(
                false, false, AcidRainEvent.OldDukeHasBeenEncountered, false,
                false, false, false, false);
            BitsByte flags11 = CreateFlags(
                false, HasGeneratedLuminitePlanetoids, downedPrimordialWyrm, downedExoMechs,
                downedAres, downedThanatos, downedArtemisAndApollo, TalkedToDraedon);
            BitsByte flags12 = CreateFlags(
                downedCragmawMire, downedMauler, downedNuclearTerror, downedBossRush,
                DraedonMechdusa, downedHorribleHog, false, false);

            writer.Write(flags);
            writer.Write(flags2);
            writer.Write(flags3);
            writer.Write(flags4);
            writer.Write(flags5);
            writer.Write(flags6);
            writer.Write(flags7);
            writer.Write(flags8);
            writer.Write(flags9);
            writer.Write(flags10);
            writer.Write(flags11);
            writer.Write(flags12);

            RecipeUnlockHandler.SendData(writer);

            writer.Write(Abyss.AbyssChasmBottom);
            writer.Write(SulphurousSea.YStart);
            writer.Write(AstralBiome.YStart);
            writer.Write(AcidRainEvent.AccumulatedKillPoints);
            writer.Write(Reforges);
            writer.Write(MoneyStolenByBandit);
            writer.Write(DraedonSummonCountdown);
            writer.Write((int)DraedonMechToSummon);
            writer.WriteVector2(DraedonSummonPosition);
            writer.WriteVector2(SunkenSeaLabCenter);
            writer.WriteVector2(PlanetoidLabCenter);
            writer.WriteVector2(JungleLabCenter);
            writer.WriteVector2(HellLabCenter);
            writer.WriteVector2(IceLabCenter);
            writer.WriteVector2(CavernLabCenter);
        }
        #endregion

        #region NetReceive
        public override void NetReceive(BinaryReader reader)
        {
            (
                downedDesertScourge, downedHiveMind, downedPerforator, downedSlimeGod,
                downedCryogen, downedBrimstoneElemental, downedCalamitasClone, downedLeviathan
            ) = ReadFlags(reader);

            (
                downedDoG, downedPlaguebringer, downedGuardians, downedProvidence,
                downedCeaselessVoid, downedStormWeaver, downedSignus, downedYharon
            ) = ReadFlags(reader);

            // Explicitly discard the now-unused vanilla boss booleans
            (
                downedCalamitas, downedDragonfolly, downedCrabulon, downedBetsy,
                downedRavager, _, _, _
            ) = ReadFlags(reader);
            (_, _, _, _, _, _, _, revenge) = ReadFlags(reader);
            (
                downedAstrumDeus, spawnedBandit, _, AcidRainEvent.HasStartedAcidicDownpour,
                _, downedPolterghast, death, downedGSS
            ) = ReadFlags(reader);
            (
                Abyss.AtLeftSideOfWorld, downedAquaticScourge, downedAstrumAureus, _,
                armageddon, _, _, _
            ) = ReadFlags(reader);
            (
                BossRushEvent.BossRushActive, downedBoomerDuke, downedCLAM, _,
                AcidRainEvent.AcidRainEventIsOngoing, downedEoCAcidRain,
                downedAquaticScourgeAcidRain, AcidRainEvent.HasTriedToSummonOldDuke
            ) = ReadFlags(reader);
            (
                AcidRainEvent.HasBeenForceStartedByEoCDefeat, _, downedSecondSentinels,
                foundHomePermafrost, downedCLAMHardMode, catName, dogName, bunnyName
            ) = ReadFlags(reader);

            reader.ReadByte();

            (_, _, AcidRainEvent.OldDukeHasBeenEncountered, _, _, _, _, _) = ReadFlags(reader);
            (
                _, HasGeneratedLuminitePlanetoids, downedPrimordialWyrm, downedExoMechs,
                downedAres, downedThanatos, downedArtemisAndApollo, TalkedToDraedon
            ) = ReadFlags(reader);

            BitsByte flags12 = reader.ReadByte();
            downedCragmawMire = flags12[0];
            downedMauler = flags12[1];
            downedNuclearTerror = flags12[2];
            downedBossRush = flags12[3];
            DraedonMechdusa = flags12[4];
            unlockedTownPig = flags12[5];
            downedHorribleHog = flags12[5];

            RecipeUnlockHandler.ReceiveData(reader);

            Abyss.AbyssChasmBottom = reader.ReadInt32();
            SulphurousSea.YStart = reader.ReadInt32();
            AstralBiome.YStart = reader.ReadInt32();
            AcidRainEvent.AccumulatedKillPoints = reader.ReadInt32();
            Reforges = reader.ReadInt32();
            MoneyStolenByBandit = reader.ReadInt32();
            DraedonSummonCountdown = reader.ReadInt32();
            DraedonMechToSummon = (ExoMech)reader.ReadInt32();
            DraedonSummonPosition = reader.ReadVector2();
            SunkenSeaLabCenter = reader.ReadVector2();
            PlanetoidLabCenter = reader.ReadVector2();
            JungleLabCenter = reader.ReadVector2();
            HellLabCenter = reader.ReadVector2();
            IceLabCenter = reader.ReadVector2();
            CavernLabCenter = reader.ReadVector2();
        }
        #endregion

        private static BitsByte CreateFlags(
            bool flag0, bool flag1, bool flag2, bool flag3,
            bool flag4, bool flag5, bool flag6, bool flag7)
        {
            BitsByte flags = new BitsByte();
            flags[0] = flag0;
            flags[1] = flag1;
            flags[2] = flag2;
            flags[3] = flag3;
            flags[4] = flag4;
            flags[5] = flag5;
            flags[6] = flag6;
            flags[7] = flag7;
            return flags;
        }

        private static (bool, bool, bool, bool, bool, bool, bool, bool) ReadFlags(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            return (
                flags[0], flags[1], flags[2], flags[3],
                flags[4], flags[5], flags[6], flags[7]);
        }
    }
}

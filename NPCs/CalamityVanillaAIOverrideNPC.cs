using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.Events;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.VanillaNPCAIOverrides;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses.BrainOfCthulhu;
using CalamityMod.NPCs.VanillaNPCAIOverrides.MiniBosses;
using CalamityMod.NPCs.VanillaNPCAIOverrides.RegularEnemies;
using CalamityMod.Systems.Collections;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.NPCs;

public static class VanillaAIOverrideExtension
{
    extension(NPC npc)
    {
        public bool TryGetAIOverride<AI>(out AI aiInstance) where AI : VanillaAIOverride, new()
        {
            if (!npc.TryGetGlobalNPC<CalamityVanillaAIOverrideNPC>(out var aiOverrideNPC))
            {
                aiInstance = null;
                return false;
            }

            aiInstance = aiOverrideNPC.AIOverride as AI;
            return aiInstance != null;
        }
    }
}

public sealed partial class CalamityVanillaAIOverrideNPC : GlobalNPC
{
    /// <summary>
    /// Toggle Entire System. External mods can toggle this out if they want.
    /// </summary>
    public static bool Enabled { get; set; } = true;

    /// <summary>
    /// Blacklist for non difficulty specific AI changes. External mods can add NPC type to opt-out global changes.
    /// <para>Example: Destroyer Probe's Telegraph Drawing</para>
    /// </summary>
    public static HashSet<int> GlobalChangeBlacklist { get; private set; } = [];

    /// <summary>
    /// Hook to Modify AI Overrides on External mods demand.<br/>
    /// Modifying <see cref="VanillaAIOverrideContext.OverrideToApply"/> will result in NPCs to use that specific AI.
    /// </summary>
    public static event Action<VanillaAIOverrideContext> ModifyAIOverride;

    /// <summary>
    /// Specify the AI Override to work with. This handles AI, SendExtraAI and ReceiveExtraAI in instaned manner.
    /// </summary>
    public VanillaAIOverride AIOverride = null;

    public static Dictionary<Type, int> NetIDLookup = [];

    public const int InvalidNetID = 0;

    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        if (entity.townNPC) return false;
        if (entity.friendly) return false;
        if (entity.CountsAsACritter) return false;
        if (CalamityNPCSets.DontCountAsEnemy[entity.type]) return false;
        return true;
    }

    #region Clone Logic
    public override bool InstancePerEntity => true;

    public override GlobalNPC Clone(NPC npc, NPC npcClone)
    {
        CalamityVanillaAIOverrideNPC clone = (CalamityVanillaAIOverrideNPC)base.Clone(npc, npcClone);
        if (AIOverride != null)
        {
            clone.AIOverride = AIOverride.Clone();
            clone.AIOverride.NPC = npcClone;
        }
        else
        {
            clone.AIOverride = null;
        }
        return clone;
    }

    #endregion

    #region Vanilla AI Override Rule
    public static VanillaAIOverride GetVanillaAIOverrideToApply(NPC npc)
    {
        if (npc == null)
            return null;

        if (npc.whoAmI < 0 || npc.whoAmI >= Main.maxNPCs)
            return null;

        if (!npc.active)
            return null;

        // Completely override the shitty AI and replace it
        if (npc.type == NPCID.BloodNautilus)
            return new DreadnautilusAI();

        // Adult Wyrm Ancient Doom
        if (npc.type == NPCID.AncientDoom)
        {
            if (Main.npc[(int)npc.ai[0]].type == NPCType<PrimordialWyrmHead>())
                return new CultistAI.AncientDoomAI();
        }

        // Zenith seed Specifics
        if (Main.zenithWorld)
        {
            if (npc.type == NPCID.QueenBee)
                return new QueenBeeAI();
        }

        // Death Mode Specifics
        if (CalamityWorld.death)
        {
            if (npc.type == NPCID.DetonatingBubble)
                return new DukeFishronAI.DetonatingBubbleAI();
        }

        #region Rev+ Mode Boss/Miniboss AI Overrides

        if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
        {
            switch (npc.type)
            {
                case NPCID.KingSlime:
                    return new KingSlimeAI();

                case NPCID.EyeofCthulhu:
                    return new EyeOfCthulhuAI();

                case NPCID.EaterofWorldsHead or NPCID.EaterofWorldsBody or NPCID.EaterofWorldsTail:
                    return new EaterOfWorldsAI();

                case NPCID.BrainofCthulhu:
                    return new BrainOfCthulhuAI();
                case NPCID.Creeper:
                    return new CreeperAI();

                case NPCID.QueenBee:
                    return new QueenBeeAI();

                case NPCID.SkeletronHead:
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        return new SkeletronAI();
                    else return null;
                
                case NPCID.SkeletronHand:
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        return new SkeletronAI.SkeletronHandAI();
                    else return null;
                
                case NPCID.DungeonGuardian:
                    return new SkeletronAI.DungeonGuardianAI();

                case NPCID.Deerclops:
                    return new DeerclopsAI();

                case NPCID.WallofFlesh:
                    return new WallOfFleshAI();
                case NPCID.WallofFleshEye:
                    return new WallOfFleshAI.EyeAI();

                case NPCID.QueenSlimeBoss:
                    return new QueenSlimeAI();
                case NPCID.QueenSlimeMinionBlue:
                    return new QueenSlimeAI.CrystalSlimeAI();
                case NPCID.QueenSlimeMinionPink:
                    return new QueenSlimeAI.BouncySlimeAI();

                case NPCID.TheDestroyer or NPCID.TheDestroyerBody or NPCID.TheDestroyerTail:
                    return new DestroyerAI();
                case NPCID.Probe:
                    return new DestroyerAI.ProbeAI();

                case NPCID.Retinazer:
                    return new TwinsAI.RetinazerAI();
                case NPCID.Spazmatism:
                    return new TwinsAI.SpazmatismAI();

                case NPCID.SkeletronPrime:
                    return new SkeletronPrimeAI();
                case NPCID.PrimeLaser:
                    return new SkeletronPrimeAI.PrimeLaserAI();
                case NPCID.PrimeCannon:
                    return new SkeletronPrimeAI.PrimeCannonAI();
                case NPCID.PrimeVice:
                    return new SkeletronPrimeAI.PrimeViceAI();
                case NPCID.PrimeSaw:
                    return new SkeletronPrimeAI.PrimeSawAI();

                case NPCID.Plantera:
                    return new PlanteraAI();
                case NPCID.PlanterasHook:
                    return new PlanteraAI.HookAI();
                case NPCID.PlanterasTentacle:
                    return new PlanteraAI.TentacleAI();

                case NPCID.HallowBoss:
                    return new EmpressofLightAI();

                case NPCID.Golem:
                    return new GolemAI();
                case NPCID.GolemFistLeft or NPCID.GolemFistRight:
                    return new GolemAI.FistAI();
                case NPCID.GolemHead:
                    return new GolemAI.HeadAI();
                case NPCID.GolemHeadFree:
                    return new GolemAI.HeadFreeAI();

                case NPCID.DukeFishron:
                    return new DukeFishronAI();

                case NPCID.Pumpking when DownedBossSystem.downedDoG:
                    return new PumpkingAI();

                case NPCID.PumpkingBlade when DownedBossSystem.downedDoG:
                    return new PumpkingAI.BladeAI();

                case NPCID.IceQueen when DownedBossSystem.downedDoG:
                    return new IceQueenAI();

                case NPCID.Mothron when DownedBossSystem.downedDoG:
                    return new MothronAI();

                case NPCID.CultistBoss or NPCID.CultistBossClone:
                    return new CultistAI();
                case NPCID.AncientLight:
                    return new CultistAI.AncientLightAI();
                case NPCID.AncientDoom:
                    return new CultistAI.AncientDoomAI();

                case NPCID.MoonLordCore:
                case NPCID.MoonLordHand:
                case NPCID.MoonLordHead:
                case NPCID.MoonLordFreeEye:
                case NPCID.MoonLordLeechBlob:
                    return new MoonLordAI();
            }
            ;
        }

        #endregion

        #region Rev+ Mode Regular Enemies AI Overrides

        if (CalamityWorld.revenge)
        {
            switch (npc.aiStyle)
            {
                case NPCAIStyleID.Slime:
                    if (npc.type == NPCType<BloomSlime>() || npc.type == NPCType<InfernalCongealment>() ||
                        npc.type == NPCType<CrimulanBlightSlime>() || npc.type == NPCType<CryoSlime>() ||
                        npc.type == NPCType<EbonianBlightSlime>() || npc.type == NPCType<PerennialSlime>() ||
                        npc.type == NPCType<IrradiatedSlime>() || npc.type == NPCType<AstralSlime>())
                    {
                        return new SlimeAI();
                    }
                    else
                    {
                        switch (npc.type)
                        {
                            case NPCID.BlueSlime:
                            case NPCID.MotherSlime:
                            case NPCID.LavaSlime:
                            case NPCID.DungeonSlime:
                            case NPCID.CorruptSlime:
                            case NPCID.IlluminantSlime:
                            case NPCID.ToxicSludge:
                            case NPCID.IceSlime:
                            case NPCID.Crimslime:
                            case NPCID.SpikedIceSlime:
                            case NPCID.SpikedJungleSlime:
                            case NPCID.UmbrellaSlime:
                            case NPCID.RainbowSlime:
                            case NPCID.SlimeMasked:
                            case NPCID.HoppinJack:
                            case NPCID.SlimeRibbonWhite:
                            case NPCID.SlimeRibbonYellow:
                            case NPCID.SlimeRibbonGreen:
                            case NPCID.SlimeRibbonRed:
                            case NPCID.SandSlime:
                            case NPCID.SlimeSpiked:
                            case NPCID.GoldenSlime:
                            case NPCID.ShimmerSlime:
                                return new SlimeAI();
                        }
                    }
                    break;

                case NPCAIStyleID.DemonEye:
                    if (npc.type == NPCType<CalamityEye>())
                    {
                        return new DemonEyeAI();
                    }
                    else
                    {
                        switch (npc.type)
                        {
                            case NPCID.DemonEye:
                            case NPCID.TheHungryII:
                            case NPCID.WanderingEye:
                            case NPCID.PigronCorruption:
                            case NPCID.PigronHallow:
                            case NPCID.PigronCrimson:
                            case NPCID.CataractEye:
                            case NPCID.SleepyEye:
                            case NPCID.DialatedEye:
                            case NPCID.GreenEye:
                            case NPCID.PurpleEye:
                            case NPCID.DemonEyeOwl:
                            case NPCID.DemonEyeSpaceship:
                                return new DemonEyeAI();
                        }
                    }
                    break;

                case NPCAIStyleID.Fighter:
                    if (npc.type == NPCType<Stormlion>() || npc.type == NPCType<BucketZombie>() ||
                        npc.type == NPCType<AstralachneaGround>() || npc.type == NPCType<RenegadeWarlock>())
                    {
                        return new RevengeanceAndDeathAI.FighterAI();
                    }
                    else if (npc.type is
                        NPCID.Zombie or NPCID.ArmedZombie or NPCID.ArmedZombieEskimo or
                        NPCID.ArmedZombiePincussion or NPCID.ArmedZombieSlimed or NPCID.ArmedZombieSwamp or
                        NPCID.ArmedZombieTwiggy or NPCID.ArmedZombieCenx or NPCID.Skeleton or NPCID.SporeSkeleton or
                        NPCID.AngryBones or NPCID.UndeadMiner or NPCID.CorruptBunny or NPCID.DoctorBones or
                        NPCID.TheGroom or NPCID.Crab or NPCID.GoblinScout or NPCID.ArmoredSkeleton or NPCID.Mummy or
                        NPCID.DarkMummy or NPCID.LightMummy or NPCID.Werewolf or NPCID.Clown or
                        NPCID.SkeletonArcher or NPCID.ChaosElemental or NPCID.BaldZombie or NPCID.PossessedArmor or
                        NPCID.ZombieEskimo or NPCID.UndeadViking or NPCID.CorruptPenguin or NPCID.FaceMonster or
                        NPCID.SnowFlinx or NPCID.PincushionZombie or NPCID.SlimedZombie or NPCID.SwampZombie or
                        NPCID.TwiggyZombie or NPCID.Nymph or NPCID.ArmoredViking or NPCID.Lihzahrd or
                        NPCID.LihzahrdCrawler or NPCID.FemaleZombie or NPCID.HeadacheSkeleton or
                        NPCID.MisassembledSkeleton or NPCID.PantlessSkeleton or NPCID.IcyMerman or
                        NPCID.PirateDeckhand or NPCID.PirateCorsair or NPCID.PirateDeadeye or
                        NPCID.PirateCrossbower or NPCID.PirateCaptain or NPCID.CochinealBeetle or
                        NPCID.CyanBeetle or NPCID.LacBeetle or NPCID.SeaSnail or NPCID.ZombieRaincoat or
                        NPCID.ZombieMushroom or NPCID.ZombieMushroomHat or NPCID.AnomuraFungus or
                        NPCID.MushiLadybug or NPCID.RustyArmoredBonesAxe or NPCID.RustyArmoredBonesFlail or
                        NPCID.RustyArmoredBonesSword or NPCID.RustyArmoredBonesSwordNoArmor or
                        NPCID.BlueArmoredBones or NPCID.BlueArmoredBonesMace or NPCID.BlueArmoredBonesNoPants or
                        NPCID.BlueArmoredBonesSword or NPCID.HellArmoredBones or
                        NPCID.HellArmoredBonesSpikeShield or NPCID.HellArmoredBonesMace or
                        NPCID.HellArmoredBonesSword or NPCID.Paladin or NPCID.SkeletonSniper or
                        NPCID.SkeletonCommando or NPCID.AngryBonesBig or NPCID.AngryBonesBigMuscle or
                        NPCID.AngryBonesBigHelmet or NPCID.Scarecrow1 or NPCID.Scarecrow2 or NPCID.Scarecrow3 or
                        NPCID.Scarecrow4 or NPCID.Scarecrow5 or NPCID.Scarecrow6 or NPCID.Scarecrow7 or
                        NPCID.Scarecrow8 or NPCID.Scarecrow9 or NPCID.Scarecrow10 or NPCID.ZombieDoctor or
                        NPCID.ZombieSuperman or NPCID.ZombiePixie or NPCID.SkeletonTopHat or
                        NPCID.SkeletonAstonaut or NPCID.SkeletonAlien or NPCID.Splinterling or NPCID.ZombieXmas or
                        NPCID.ZombieSweater or NPCID.ZombieElf or NPCID.ZombieElfBeard or NPCID.ZombieElfGirl or
                        NPCID.GingerbreadMan or NPCID.Yeti or NPCID.Nutcracker or NPCID.NutcrackerSpinning or
                        NPCID.ElfArcher or NPCID.Krampus or NPCID.CultistArcherBlue or NPCID.CultistArcherWhite or
                        NPCID.BrainScrambler or NPCID.RayGunner or NPCID.MartianOfficer or NPCID.GrayGrunt or
                        NPCID.MartianEngineer or NPCID.GigaZapper or NPCID.Scutlix or NPCID.BoneThrowingSkeleton or
                        NPCID.BoneThrowingSkeleton2 or NPCID.BoneThrowingSkeleton3 or NPCID.BoneThrowingSkeleton4 or
                        NPCID.CrimsonBunny or NPCID.CrimsonPenguin or NPCID.Medusa or NPCID.GreekSkeleton or
                        NPCID.GraniteGolem or NPCID.BloodZombie or NPCID.Crawdad or NPCID.Crawdad2 or
                        NPCID.Salamander or NPCID.Salamander2 or NPCID.Salamander3 or NPCID.Salamander4 or
                        NPCID.Salamander5 or NPCID.Salamander6 or NPCID.Salamander7 or NPCID.Salamander8 or
                        NPCID.Salamander9 or NPCID.GiantWalkingAntlion or NPCID.WalkingAntlion or
                        NPCID.LarvaeAntlion or NPCID.DesertGhoul or NPCID.DesertGhoulCorruption or
                        NPCID.DesertGhoulCrimson or NPCID.DesertGhoulHallow or NPCID.DesertLamiaLight or
                        NPCID.DesertLamiaDark or NPCID.DesertScorpionWalk or NPCID.DesertBeast or
                        NPCID.StardustSoldier or NPCID.StardustSpiderBig or NPCID.NebulaSoldier or
                        NPCID.VortexSoldier or NPCID.SolarDrakomire or NPCID.SolarSpearman or NPCID.SolarSolenian or
                        NPCID.Frankenstein or NPCID.SwampThing or NPCID.Vampire or NPCID.Butcher or
                        NPCID.CreatureFromTheDeep or NPCID.Fritz or NPCID.Psycho or NPCID.ThePossessed or
                        NPCID.DrManFly or NPCID.GoblinPeon or NPCID.GoblinThief or NPCID.GoblinWarrior or
                        NPCID.GoblinArcher or NPCID.GoblinSummoner or NPCID.MartianWalker or
                        NPCID.DemonTaxCollector or NPCID.TheBride)
                    {
                        return new RevengeanceAndDeathAI.FighterAI();
                    }
                    break;

                case NPCAIStyleID.Flying:
                    switch (npc.type)
                    {
                        case NPCID.ServantofCthulhu:
                        case NPCID.EaterofSouls:
                        case NPCID.MeteorHead:
                        case NPCID.Crimera:
                        case NPCID.Moth:
                        case NPCID.Parrot:
                        case NPCID.Bee:
                        case NPCID.BeeSmall:
                        case NPCID.Hornet:
                        case NPCID.HornetFatty:
                        case NPCID.HornetHoney:
                        case NPCID.HornetLeafy:
                        case NPCID.HornetSpikey:
                        case NPCID.HornetStingy:
                        case NPCID.MossHornet:
                            return new RevengeanceAndDeathAI.FlyingAI();
                    }
                    break;

                case NPCAIStyleID.Worm:
                    switch (npc.type)
                    {
                        case NPCID.DevourerHead:
                        case NPCID.DevourerBody:
                        case NPCID.DevourerTail:
                        case NPCID.GiantWormHead:
                        case NPCID.GiantWormBody:
                        case NPCID.GiantWormTail:
                        case NPCID.BoneSerpentHead:
                        case NPCID.BoneSerpentBody:
                        case NPCID.BoneSerpentTail:
                        case NPCID.WyvernHead:
                        case NPCID.WyvernLegs:
                        case NPCID.WyvernBody:
                        case NPCID.WyvernBody2:
                        case NPCID.WyvernBody3:
                        case NPCID.WyvernTail:
                        case NPCID.LeechHead:
                        case NPCID.LeechBody:
                        case NPCID.LeechTail:
                        case NPCID.TombCrawlerHead:
                        case NPCID.TombCrawlerBody:
                        case NPCID.TombCrawlerTail:
                        case NPCID.StardustWormHead:
                        case NPCID.SolarCrawltipedeHead:
                        case NPCID.SolarCrawltipedeBody:
                        case NPCID.SolarCrawltipedeTail:
                        case NPCID.BloodEelHead:
                        case NPCID.BloodEelBody:
                        case NPCID.BloodEelTail:
                            return new RevengeanceAndDeathAI.WormAI();
                    }
                    break;

                case NPCAIStyleID.ManEater:
                    switch (npc.type)
                    {
                        case NPCID.ManEater:
                        case NPCID.Snatcher:
                        case NPCID.Clinger:
                        case NPCID.AngryTrapper:
                        case NPCID.FungiBulb:
                        case NPCID.GiantFungiBulb:
                            return new RevengeanceAndDeathAI.PlantAI();
                    }
                    break;

                case NPCAIStyleID.Bat:
                    if (npc.type == NPCType<StellarCulex>() || npc.type == NPCType<Melter>() || npc.type == NPCType<AeroSlime>())
                    {
                        return new RevengeanceAndDeathAI.BatAI();
                    }
                    else
                    {
                        switch (npc.type)
                        {
                            case NPCID.CaveBat:
                            case NPCID.JungleBat:
                            case NPCID.Hellbat:
                            case NPCID.GiantBat:
                            case NPCID.Slimer:
                            case NPCID.IlluminantBat:
                            case NPCID.IceBat:
                            case NPCID.Lavabat:
                            case NPCID.GiantFlyingFox:
                            case NPCID.FlyingSnake:
                            case NPCID.VampireBat:
                            case NPCID.SporeBat:
                                return new RevengeanceAndDeathAI.BatAI();
                        }
                    }
                    break;

                case NPCAIStyleID.Piranha:
                    switch (npc.type)
                    {
                        case NPCID.CorruptGoldfish:
                        case NPCID.Piranha:
                        case NPCID.Shark:
                        case NPCID.AnglerFish:
                        case NPCID.Arapaima:
                        case NPCID.BloodFeeder:
                        case NPCID.CrimsonGoldfish:
                            return new RevengeanceAndDeathAI.SwimmingAI();
                    }
                    break;

                case NPCAIStyleID.Jellyfish:
                    switch (npc.type)
                    {
                        case NPCID.BlueJellyfish:
                        case NPCID.PinkJellyfish:
                        case NPCID.GreenJellyfish:
                        case NPCID.Squid:
                        case NPCID.BloodJelly:
                        case NPCID.FungoFish:
                            return new RevengeanceAndDeathAI.JellyfishAI();
                    }
                    break;

                case NPCAIStyleID.SpikeBall:
                    switch (npc.type)
                    {
                        case NPCID.SpikeBall:
                            return new RevengeanceAndDeathAI.SpikeBallAI();
                    }
                    break;

                case NPCAIStyleID.BlazingWheel:
                    switch (npc.type)
                    {
                        case NPCID.BlazingWheel:
                            return new RevengeanceAndDeathAI.BlazingWheelAI();
                    }
                    break;

                case NPCAIStyleID.HoveringFighter:
                    switch (npc.type)
                    {
                        case NPCID.Pixie:
                        case NPCID.Wraith:
                        case NPCID.Gastropod:
                        case NPCID.FloatyGross:
                        case NPCID.Ghost:
                        case NPCID.Poltergeist:
                        case NPCID.Drippler:
                        case NPCID.Reaper:
                            return new RevengeanceAndDeathAI.HoveringAI();
                    }
                    break;

                case NPCAIStyleID.EnchantedSword:
                    switch (npc.type)
                    {
                        case NPCID.CursedHammer:
                        case NPCID.EnchantedSword:
                        case NPCID.CrimsonAxe:
                            return new RevengeanceAndDeathAI.FlyingWeaponAI();
                    }
                    break;

                case NPCAIStyleID.Mimic:
                    switch (npc.type)
                    {
                        case NPCID.Mimic:
                        case NPCID.PresentMimic:
                        case NPCID.IceMimic:
                            return new RevengeanceAndDeathAI.MimicAI();
                    }
                    break;

                case NPCAIStyleID.Unicorn:
                    if (npc.type == NPCType<Rotdog>())
                    {
                        return new RevengeanceAndDeathAI.UnicornAI();
                    }
                    else
                    {
                        switch (npc.type)
                        {
                            case NPCID.Unicorn:
                            case NPCID.Wolf:
                            case NPCID.HeadlessHorseman:
                            case NPCID.Hellhound:
                            case NPCID.StardustSpiderSmall:
                            case NPCID.NebulaBeast:
                            case NPCID.Tumbleweed:
                                return new RevengeanceAndDeathAI.UnicornAI();
                        }
                    }
                    break;

                case NPCAIStyleID.TheHungry:
                    switch (npc.type)
                    {
                        case NPCID.TheHungry:
                            return new WallOfFleshAI.HungryAI();
                    }
                    break;

                case NPCAIStyleID.GiantTortoise:
                    if (npc.type == NPCType<Plagueshell>())
                    {
                        return new RevengeanceAndDeathAI.TortoiseAI();
                    }
                    else
                    {
                        switch (npc.type)
                        {
                            case NPCID.GiantTortoise:
                            case NPCID.IceTortoise:
                            case NPCID.GiantShelly:
                            case NPCID.GiantShelly2:
                            case NPCID.SolarSroller:
                                return new RevengeanceAndDeathAI.TortoiseAI();
                        }
                    }
                    break;

                case NPCAIStyleID.Spider:
                    switch (npc.type)
                    {
                        case NPCID.DesertScorpionWall:
                            return new RevengeanceAndDeathAI.SpiderAI();
                    }
                    break;

                case NPCAIStyleID.Herpling:
                    if (npc.type == NPCType<Aries>())
                    {
                        return new RevengeanceAndDeathAI.HerplingAI();
                    }
                    else
                    {
                        switch (npc.type)
                        {
                            case NPCID.Herpling:
                            case NPCID.Derpling:
                            case NPCID.ChatteringTeethBomb:
                                return new RevengeanceAndDeathAI.HerplingAI();
                        }
                    }
                    break;

                case NPCAIStyleID.FlyingFish:
                    switch (npc.type)
                    {
                        case NPCID.FlyingFish:
                        case NPCID.GiantFlyingAntlion:
                        case NPCID.FlyingAntlion:
                        case NPCID.EyeballFlyingFish:
                            return new RevengeanceAndDeathAI.FlyingFishAI();
                    }
                    break;

                case NPCAIStyleID.AngryNimbus:
                    switch (npc.type)
                    {
                        case NPCID.AngryNimbus:
                            return new RevengeanceAndDeathAI.AngryNimbusAI();
                    }
                    break;

                case NPCAIStyleID.TeslaTurret:
                    switch (npc.type)
                    {
                        case NPCID.MartianTurret:
                            return new RevengeanceAndDeathAI.TeslaTurretAI();
                    }
                    break;

                case NPCAIStyleID.Corite:
                    switch (npc.type)
                    {
                        case NPCID.MartianDrone:
                        case NPCID.SolarCorite:
                            return new RevengeanceAndDeathAI.CoriteAI();
                    }
                    break;

                case NPCAIStyleID.MartianProbe:
                    switch (npc.type)
                    {
                        case NPCID.MartianProbe:
                            return new RevengeanceAndDeathAI.MartianProbeAI();
                    }
                    break;

                case NPCAIStyleID.StarCell:
                    switch (npc.type)
                    {
                        case NPCID.StardustCellBig:
                        case NPCID.NebulaHeadcrab:
                        case NPCID.DeadlySphere:
                            return new RevengeanceAndDeathAI.StarCellAI();
                    }
                    break;

                case NPCAIStyleID.AncientVision:
                    switch (npc.type)
                    {
                        case NPCID.ShadowFlameApparition:
                        case NPCID.AncientCultistSquidhead:
                            return new RevengeanceAndDeathAI.AncientVisionAI();
                    }
                    break;

                case NPCAIStyleID.BiomeMimic:
                    switch (npc.type)
                    {
                        case NPCID.BigMimicCorruption:
                        case NPCID.BigMimicCrimson:
                        case NPCID.BigMimicHallow:
                        case NPCID.BigMimicJungle:
                            return new RevengeanceAndDeathAI.BigMimicAI();
                    }
                    break;

                case NPCAIStyleID.MothronEgg:
                    switch (npc.type)
                    {
                        case NPCID.MothronEgg:
                            return new RevengeanceAndDeathAI.MothronEggAI();
                    }
                    break;

                case NPCAIStyleID.GraniteElemental:
                    switch (npc.type)
                    {
                        case NPCID.GraniteFlyer:
                            return new RevengeanceAndDeathAI.GraniteElementalAI();
                    }
                    break;

                case NPCAIStyleID.SmallStarCell:
                    switch (npc.type)
                    {
                        case NPCID.StardustCellSmall:
                            return new RevengeanceAndDeathAI.SmallStarCellAI();
                    }
                    break;

                case NPCAIStyleID.FlowInvader:
                    switch (npc.type)
                    {
                        case NPCID.StardustJellyfishBig:
                            return new RevengeanceAndDeathAI.FlowInvaderAI();
                    }
                    break;

                case NPCAIStyleID.Spore:
                    switch (npc.type)
                    {
                        case NPCID.Spore:
                        case NPCID.FungiSpore:
                            return new RevengeanceAndDeathAI.SporeAI();
                    }
                    break;
            }
        }

        #endregion

        return null;
    }
    #endregion

    internal static bool IsGlobalChangeBlacklisted(NPC npc) => GlobalChangeBlacklist.Contains(npc.type);

    internal static void RegisterNetID(VanillaAIOverride aiOverride)
    {
        var id = NetIDLookup.Count + 1;
        NetIDLookup[aiOverride.GetType()] = id;
    }

    public override void Unload()
    {
        NetIDLookup.Clear();
        GlobalChangeBlacklist.Clear();
        ModifyAIOverride = null;
    }

    public override void SetDefaults(NPC npc)
    {
        if (!Enabled)
            return;

        // Clients will get their instance in ReceiveExtraAI
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

        AIOverride = GetVanillaAIOverrideToApply(npc);
        if (ModifyAIOverride != null)
        {
            var context = new VanillaAIOverrideContext()
            {
                NPC = npc,
                NPCType = npc.type,
                InRevengeanceWorld = CalamityWorld.revenge,
                InDeathWorld = CalamityWorld.death,
                InBossRush = BossRushEvent.BossRushActive,
                OverrideToApply = AIOverride
            };
            ModifyAIOverride.Invoke(context);
            AIOverride = context.OverrideToApply;
        }

        if (AIOverride != null)
        {
            AIOverride.NPC = npc;
            AIOverride.SetDefaults(Mod);
        }
    }

    #region Hooks

    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        if (!Enabled)
            return;

        AIOverride?.OnSpawn(Mod);
    }

    public override bool PreAI(NPC npc)
    {
        if (!Enabled)
            return base.PreAI(npc);

        bool result = true;
        if (!IsGlobalChangeBlacklisted(npc)) result &= GlobalPreAI(npc);
        if (AIOverride != null)
        {
            result &= AIOverride.AI(Mod);

            if (AIOverride.DisableMultiplayerSmoothing)
            {
                npc.netOffset = Vector2.Zero;
                if (AIOverride.EnableMultiplayerSmoothingAheadOfAI)
                    AIOverride.DisableMultiplayerSmoothing = false;
            }
        }
        return result;
    }

    public override void AI(NPC npc)
    {
        if (!Enabled)
            return;

        if (!IsGlobalChangeBlacklisted(npc)) GlobalAI(npc);
    }

    public override void PostAI(NPC npc)
    {
        if (!Enabled)
            return;

        AIOverride?.PostAI(Mod);
    }

    public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
    {
        if (!Enabled)
            return base.CanBeHitByProjectile(npc, projectile);

        return AIOverride?.CanBeHitByProjectile(Mod, projectile);
    }

    public override void HitEffect(NPC npc, NPC.HitInfo hit)
    {
        if (!Enabled)
            return;

        AIOverride?.HitEffect(Mod, hit);
    }

    public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
    {
        if (!Enabled)
            return;

        AIOverride?.ModifyHitByItem(Mod, player, item, ref modifiers);
    }

    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (!Enabled)
            return;

        AIOverride?.ModifyHitByProjectile(Mod, projectile, ref modifiers);
    }

    public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
    {
        if (!Enabled)
            return;

        AIOverride?.OnHitByItem(Mod, player, item, hit, damageDone);
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
    {
        if (!Enabled)
            return;

        AIOverride?.OnHitByProjectile(Mod, projectile, hit, damageDone);
    }

    public override bool PreKill(NPC npc)
    {
        if (!Enabled || AIOverride == null)
            return base.PreKill(npc);

        return AIOverride.PreKill(Mod);
    }

    public override void FindFrame(NPC npc, int frameHeight)
    {
        if (!Enabled || npc.IsABestiaryIconDummy)
            return;

        AIOverride?.FindFrame(Mod, frameHeight);
    }

    public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!Enabled)
            base.PreDraw(npc, spriteBatch, screenPos, drawColor);

        if(npc.IsABestiaryIconDummy)
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);

        bool result = true;
        if (!IsGlobalChangeBlacklisted(npc)) result &= GlobalPreDraw(npc, spriteBatch, screenPos, drawColor);
        result &= AIOverride?.PreDraw(Mod, spriteBatch, screenPos, drawColor) ?? true;
        return result;
    }

    public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!Enabled)
            return;

        if (!IsGlobalChangeBlacklisted(npc)) GlobalPostDraw(npc, spriteBatch, screenPos, drawColor);
        AIOverride?.PostDraw(Mod, spriteBatch, screenPos, drawColor);
    }

    #endregion

    #region Networking

    public static int GetNetID(VanillaAIOverride aiOverride)
    {
        if (aiOverride == null)
            return InvalidNetID;

        if (!NetIDLookup.TryGetValue(aiOverride.GetType(), out var netID))
            return InvalidNetID;

        return netID;
    }

    public static bool TryGetNetID(VanillaAIOverride aIOverride, out int netID)
    {
        netID = GetNetID(aIOverride);
        return netID != InvalidNetID;
    }

    public static VanillaAIOverride GetNewInstanceOrNullFromNetID(int netID, NPC ownerNPC)
    {
        var type = NetIDLookup.FirstOrDefault(kv => kv.Value == netID).Key;

        if (type == null)
            return null;

        var instance = (VanillaAIOverride)Activator.CreateInstance(type);
        if (instance == null)
            return null;

        instance.NPC = ownerNPC;
        return instance;
    }

    public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
    {
        // OnKill or any similar hooks are not reliable for checking these.
        // As SetDefaults being called on deactivated/dead NPC before ReceiveExtraAI, Prevent Sending ExtraAI is only clean way to do.
        if (!npc.active || npc.life <= 0)
        {
            AIOverride = null;
            binaryWriter.Write7BitEncodedInt(InvalidNetID);
            return;
        }

        if (!TryGetNetID(AIOverride, out var netID))
        {
            binaryWriter.Write7BitEncodedInt(InvalidNetID);
            return;
        }

        binaryWriter.Write7BitEncodedInt(netID);
        AIOverride.SendExtraAI(bitWriter, binaryWriter);
    }

    public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
    {
        var remoteNetID = binaryReader.Read7BitEncodedInt();
        var localNetID = GetNetID(AIOverride);
        if (localNetID != remoteNetID)
        {
            AIOverride = GetNewInstanceOrNullFromNetID(remoteNetID, npc);
            AIOverride?.SetDefaults(Mod);
        }

        AIOverride?.ReceiveExtraAI(bitReader, binaryReader);
    }

    #endregion
}

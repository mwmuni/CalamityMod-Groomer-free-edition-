using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.CalPlayer;

namespace Microsoft.Xna.Framework
{
    public struct Vector2(float x, float y)
    {
        public float X = x, Y = y;
        public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
    }

    public struct Color(byte r, byte g, byte b)
    {
        public byte R = r, G = g, B = b;
    }

    public struct Point { public static Point Zero => default; }
}

namespace Terraria
{
    using Microsoft.Xna.Framework;

    public static class Main
    {
        public const int maxPlayers = 255, maxNPCs = 200;
        public static int netMode, myPlayer;
        public static uint GameUpdateCount;
        public static bool dedServ => netMode == ID.NetmodeID.Server;
        public static Player[] player = Enumerable.Range(0, maxPlayers).Select(i => new Player { whoAmI = i }).ToArray();
        public static Player LocalPlayer => player[myPlayer];
        public static NPC[] npc = Enumerable.Range(0, maxNPCs).Select(i => new NPC { whoAmI = i }).ToArray();
        public static IEnumerable<Player> ActivePlayers => player.Where(p => p.active);
    }

    public class Player
    {
        public int whoAmI;
        public bool active;
        public Vector2 position;
        public CalamityPlayer ModPlayer;
    }

    public class NPC
    {
        public int whoAmI, type;
        public bool active;
        public Vector2 position;
        public float rotation;
        public float[] localAI = new float[4];
        public ModLoader.ModNPC ModNPC;
        public CalamityMod.NPCs.CalamityGlobalNPC CalamityData = new();
        public static int NewNPC(DataStructures.EntitySource_WorldEvent source, int x, int y, int type, int Target) => 0;
    }

    public struct BitsByte
    {
        private byte value;
        public bool this[int index]
        {
            readonly get => (value & (1 << index)) != 0;
            set => this.value = (byte)(value ? this.value | (1 << index) : this.value & ~(1 << index));
        }
        public static implicit operator byte(BitsByte bits) => bits.value;
        public static implicit operator BitsByte(byte value) => new() { value = value };
    }

    public static class NetMessage
    {
        public static List<int> SentMessageIDs = [];
        public static Action<int> OnSend;
        public static void SendData(int id, int toClient = -1, int ignoreClient = -1, object text = null, int number = 0)
        {
            SentMessageIDs.Add(id);
            OnSend?.Invoke(id);
        }
    }
}

namespace Terraria.ID
{
    public static class NetmodeID { public const int SinglePlayer = 0, MultiplayerClient = 1, Server = 2; }
    public static class MessageID { public const int WorldData = 7, SyncNPC = 23; }
    public static class NPCID
    {
        public const int EaterofWorldsHead = 13, EaterofWorldsBody = 14, EaterofWorldsTail = 15;
        public const int TheDestroyer = 134, TheDestroyerBody = 135, TheDestroyerTail = 136;
    }
}

namespace Terraria.DataStructures
{
    public class EntitySource_WorldEvent { }
    public class TileEntity
    {
        public int ID;
        public static Dictionary<int, TileEntity> ByID = [];
    }
}

namespace Terraria.ModLoader
{
    public interface ILoadable { void Load(Mod mod); void Unload(); }
    public class Mod { public ModPacket GetPacket() => new(); }
    public record SentPacket(byte[] Bytes, int ToClient, int IgnoreClient);
    public class ModPacket() : BinaryWriter(new MemoryStream())
    {
        public static List<SentPacket> Sent = [];
        public void Send(int toClient = -1, int ignoreClient = -1) =>
            Sent.Add(new(((MemoryStream)BaseStream).ToArray(), toClient, ignoreClient));
    }
    public class ModSystem
    {
        public virtual void OnModUnload() { }
        public virtual void OnWorldUnload() { }
        public virtual void PostUpdateEverything() { }
        public virtual void ClearWorld() { }
        public virtual void SaveWorldData(IO.TagCompound tag) { }
        public virtual void LoadWorldData(IO.TagCompound tag) { }
        public virtual void NetSend(BinaryWriter writer) { }
        public virtual void NetReceive(BinaryReader reader) { }
    }
    public class ModPlayer
    {
        public Terraria.Player Player;
        public virtual void SyncPlayer(int toWho, int fromWho, bool newPlayer) { }
    }
    public class ModNPC
    {
        public Terraria.NPC NPC;
        public int Type;
        public string FullName => GetType().FullName;
    }
    public class GlobalNPC
    {
        public virtual void Load() { }
        public virtual void Unload() { }
        public virtual void SetStaticDefaults() { }
        public virtual void PostAI(Terraria.NPC npc) { }
    }
    public static class ModContent { public static IEnumerable<T> GetContent<T>() => []; public static int NPCType<T>() => 0; }
}

namespace Terraria.ModLoader.IO
{
    public class TagCompound : Dictionary<string, object>
    {
        public IList<T> GetList<T>(string key) => (IList<T>)this[key];
        public int GetInt(string key) => (int)this[key];
        public Vector Get<Vector>(string key) => (Vector)this[key];
    }
}

namespace CalamityMod
{
    using Microsoft.Xna.Framework;
    public class TestLog
    {
        public List<string> Errors = [], Warnings = [];
        public void Error(string message, Exception exception = null) => Errors.Add(message);
        public void Warn(string message) => Warnings.Add(message);
    }
    public static class CalamityMod
    {
        public static readonly Terraria.ModLoader.Mod Instance = new();
        public static readonly TestLog Log = new();
    }
    public static partial class CalamityUtils
    {
        public static CalamityPlayer Calamity(this Terraria.Player player) => player.ModPlayer;
        public static NPCs.CalamityGlobalNPC Calamity(this Terraria.NPC npc) => npc.CalamityData;
        public static bool IsNullOrInactive(this Terraria.Player player) => player is null || !player.active;
        public static bool IsNullOrInactive(this Terraria.NPC npc) => npc is null || !npc.active;
        public static float ManhattanDistance(Vector2 a, Vector2 b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        public static float ToRotation(this Vector2 vector) => MathF.Atan2(vector.Y, vector.X);
        public static Vector2 ToRotationVector2(this float angle) => new(MathF.Cos(angle), MathF.Sin(angle));
        public static void WriteVector2(this BinaryWriter writer, Vector2 value) { writer.Write(value.X); writer.Write(value.Y); }
        public static Vector2 ReadVector2(this BinaryReader reader) => new(reader.ReadSingle(), reader.ReadSingle());
        public static void WriteRGB(this BinaryWriter writer, Color value) { writer.Write(value.R); writer.Write(value.G); writer.Write(value.B); }
        public static Color ReadRGB(this BinaryReader reader) => new(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
        public static void AddWithCondition(this List<string> list, string value, bool condition) { if (condition) list.Add(value); }
    }
    public static class DownedBossSystem
    {
        public static bool downedDesertScourge, downedHiveMind, downedPerforator, downedSlimeGod,
            downedCryogen, downedBrimstoneElemental, downedCalamitasClone, downedLeviathan,
            downedDoG, downedPlaguebringer, downedGuardians, downedProvidence,
            downedCeaselessVoid, downedStormWeaver, downedSignus, downedYharon,
            downedCalamitas, downedDragonfolly, downedCrabulon, downedBetsy, downedRavager,
            downedAstrumDeus, downedPolterghast, downedGSS, downedAquaticScourge, downedAstrumAureus,
            downedBoomerDuke, downedCLAM, downedEoCAcidRain, downedAquaticScourgeAcidRain,
            downedSecondSentinels, downedCLAMHardMode, downedPrimordialWyrm, downedExoMechs,
            downedAres, downedThanatos, downedArtemisAndApollo, downedCragmawMire, downedMauler,
            downedNuclearTerror, downedBossRush, downedHorribleHog;
        public static void ResetAllFlags() { }
    }
}

namespace CalamityMod.CalPlayer
{
    public struct CalamityPlayerDrawingParameters
    {
        public float RoverShieldCharge, LunicShieldCharge, ProfanedShieldCharge, SpongeShieldCharge;
        public Microsoft.Xna.Framework.Color ProfanedShieldColor;
    }
    public partial class CalamityPlayer
    {
        public float rage, adrenaline, mouseRotationFromPlayer;
        public int rageCombatFrames, totalDefenseDamage, defenseDamageRecoveryFrames, totalDefenseDamageRecoveryFrames, defenseDamageDelayFrames;
        public Microsoft.Xna.Framework.Vector2 mouseWorldDeltaFromPlayer;
        public bool mouseRight, InvertExaltationLineRotationDirections;
        public int RoverDriveShieldDurability, LunicCorpsShieldDurability, pSoulShieldDurability, SpongeShieldDurability;
        public CalamityPlayerDrawingParameters drawingParameters;
        public Dictionary<string, Cooldowns.CooldownInstance> cooldowns = [];
    }
}

namespace CalamityMod.Cooldowns
{
    public class Cooldown(string id, ushort netID) { public string ID = id; public ushort netID = netID; }
    public static class CooldownRegistry
    {
        public static Cooldown[] registry = [new("one", 0), new("two", 1), new("three", 2)];
        public static Cooldown Get(string id) => registry.Single(cd => cd.ID == id);
    }
    public class CooldownInstance(Terraria.Player player, ushort id, int duration, int timeLeft)
    {
        public Terraria.Player player = player;
        public ushort netID = id;
        public int duration = duration, timeLeft = timeLeft;
    }
}

namespace CalamityMod.NPCs
{
    public class CalamityGlobalNPC
    {
        public float[] newAI = new float[4];
        public static int holyBoss, doughnutBoss, doughnutBossDefender, doughnutBossHealer, voidBoss,
            energyFlame, hiveMind, astrumAureus, scavenger, DoGHead, SCal, ghostBoss, laserEye, fireEye,
            brimstoneElemental, signus, draedonExoMechPrimePlasmaCannon, draedonExoMechPrime,
            draedonExoMechTwinGreen, draedonExoMechTwinRed, draedonExoMechWorm, adultEidolonWyrmHead;
        public static int[] bobbitWormBottom = [];
    }
}

namespace CalamityMod.TileEntities
{
    public class TEPowerCellFactory : Terraria.DataStructures.TileEntity { public long Time; public short Stack_Internal; }
    public class TECodebreaker : Terraria.DataStructures.TileEntity
    {
        public int InputtedCellCount, InitialCellCountBeforeDecrypting, HeldSchematicID, DecryptionCountdown;
        public bool ContainsDecryptionComputer, ContainsSensorArray, ContainsAdvancedDisplay,
            ContainsVoltageRegulationSystem, ContainsCoolingCell, ContainsBloodyVein;
    }
}

namespace CalamityMod.Packets
{
    public static class SpawnNPCOnPlayerPacket
    {
        public static void Send(Terraria.Player player, int x, int y, int type) { }
    }
}

namespace CalamityMod.World
{
    using Microsoft.Xna.Framework;
    public enum ExoMech { None, Ares, Thanatos, ArtemisAndApollo }
    public static class CalamityWorld
    {
        public static bool spawnedBandit, foundHomePermafrost, unlockedTownPig, catName, dogName, bunnyName,
            revenge, TalkedToDraedon, death, armageddon, HasGeneratedLuminitePlanetoids,
            IsWorldAfterDraedonUpdate, DraedonMechdusa;
        public static int Reforges, MoneyStolenByBandit, DraedonSummonCountdown;
        public static ExoMech DraedonMechToSummon;
        public static Vector2 DraedonSummonPosition, SunkenSeaLabCenter, PlanetoidLabCenter,
            JungleLabCenter, HellLabCenter, IceLabCenter, CavernLabCenter;
    }
    public static class Abyss { public static int AbyssChasmBottom; public static bool AtLeftSideOfWorld; }
    public static class SulphurousSea { public static int YStart; }
    public static class AstralBiome { public static int YStart; }
    public static class CustomTemple { public static Point NewAlterPosition; }
}

namespace CalamityMod.Events
{
    public static class AcidRainEvent
    {
        public static bool AcidRainEventIsOngoing, HasTriedToSummonOldDuke, HasStartedAcidicDownpour,
            HasBeenForceStartedByEoCDefeat, OldDukeHasBeenEncountered;
        public static int CountdownUntilForcedAcidRain, AccumulatedKillPoints;
    }
    public static class BossRushEvent
    {
        public static bool BossRushActive;
        public static int BossRushStage, BossRushSpawnCountdown, HostileProjectileKillCounter;
    }
}

namespace CalamityMod.CustomRecipes
{
    public static class RecipeUnlockHandler
    {
        public static void Save(List<string> flags) { }
        public static void Load(IList<string> flags) { }
        public static void SendData(BinaryWriter writer) { }
        public static void ReceiveData(BinaryReader reader) { }
    }
}

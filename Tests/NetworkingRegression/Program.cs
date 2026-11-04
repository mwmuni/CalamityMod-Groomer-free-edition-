using System;
using System.IO;
using System.Linq;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Debugging;
using CalamityMod.NPCs;
using CalamityMod.Packets;
using CalamityMod.TileEntities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

internal static class Program
{
    private static int assertions;

    private static void Main()
    {
        foreach (var type in typeof(Program).Assembly.GetTypes().Where(t => !t.IsAbstract && typeof(CalamityPacket).IsAssignableFrom(t)))
            ((CalamityPacket)Activator.CreateInstance(type)).Load(CalamityMod.CalamityMod.Instance);

        TestPlayerTraffic();
        TestCooldownSnapshots();
        TestInvalidCooldownAdditions();
        TestRelays();
        TestNPCSnapshots();
        TestWorldFlags();
        TestWorldSyncBatching();
        TestPacketDiagnostics();
        TestPacketRegistry();
        Console.WriteLine($"PASS: {assertions} networking assertions.");
    }

    private static CalamityPlayer PlayerAt(int index)
    {
        var player = Terraria.Main.player[index];
        player.active = true;
        return player.ModPlayer = new CalamityPlayer { Player = player };
    }

    private static void Check(bool condition, string message)
    {
        assertions++;
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private static void Throws<T>(Action action, string message) where T : Exception
    {
        try { action(); }
        catch (T) { assertions++; return; }
        throw new InvalidOperationException(message);
    }

    private static BinaryReader Payload(SentPacket sent)
    {
        var reader = new BinaryReader(new MemoryStream(sent.Bytes));
        CalamityNetcode.ReadHandlerNetID(reader);
        return reader;
    }

    private static void Deliver(SentPacket sent, int sender)
    {
        using var reader = new BinaryReader(new MemoryStream(sent.Bytes));
        CalamityNetcode.HandlePacket(CalamityMod.CalamityMod.Instance, reader, sender);
        Check(reader.BaseStream.Position == reader.BaseStream.Length, "Packet must consume its complete payload.");
    }

    private static void TestPlayerTraffic()
    {
        Terraria.Main.netMode = NetmodeID.MultiplayerClient;
        Terraria.Main.myPlayer = 0;
        var player = PlayerAt(0);
        ModPacket.Sent.Clear();
        player.StandardSync();
        Check(ModPacket.Sent.Count == 1, "Initial combat stats must be sent atomically in one packet.");

        ModPacket.Sent.Clear();
        for (int i = 0; i < 80; i++)
            player.StandardSync();
        Check(ModPacket.Sent.Count == 10, "Idle stats must use 10 packets over 1,200 ticks, not 240 (95.8% fewer).");

        ModPacket.Sent.Clear();
        player.rage = 10;
        player.adrenaline = 20;
        player.totalDefenseDamage = 5;
        for (int i = 0; i < 8; i++)
            player.StandardSync();
        Check(ModPacket.Sent.Count == 8, "Active/capped stats must retain all 15-tick corrections in one packet.");

        ModPacket.Sent.Clear();
        player.rage = player.adrenaline = 0;
        player.totalDefenseDamage = 0;
        player.StandardSync();
        Check(ModPacket.Sent.Count == 1, "Returning to zero must sync immediately on the next stat check.");

        var remote = PlayerAt(1);
        player.rage = 35f;
        player.rageCombatFrames = 45;
        player.adrenaline = 62f;
        player.totalDefenseDamage = 8;
        player.defenseDamageRecoveryFrames = 90;
        player.totalDefenseDamageRecoveryFrames = 180;
        player.defenseDamageDelayFrames = 15;
        ModPacket.Sent.Clear();
        CombatStatsSyncPacket.Send(player);
        var stats = ModPacket.Sent.Single();
        Check(stats.Bytes.Length == 30, "The combined stat packet must use one header for all seven synchronized values.");
        Terraria.Main.netMode = NetmodeID.Server;
        player.rage = player.adrenaline = 0f;
        player.rageCombatFrames = player.totalDefenseDamage = 0;
        player.defenseDamageRecoveryFrames = player.defenseDamageDelayFrames = 0;
        ModPacket.Sent.Clear();
        Deliver(stats, 0);
        Check(player.rage == 35f && player.adrenaline == 62f && player.totalDefenseDamage == 8,
            "The server must apply all combat values from one atomic update.");
        Check(ModPacket.Sent.Single().IgnoreClient == 0, "The server must relay the combined stat update to other peers.");

        ModPacket.Sent.Clear();
        var forgedStats = stats;
        Deliver(forgedStats, 1);
        Check(player.rage == 35f && ModPacket.Sent.Count == 0, "Clients must not update another player's combat stats.");

        Terraria.Main.netMode = NetmodeID.MultiplayerClient;
        ModPacket.Sent.Clear();
        remote.SyncCooldownAddition(false, new CooldownInstance(remote.Player, 0, 100, 100));
        remote.SyncCooldownRemoval(false, ["one"]);
        remote.SyncCooldownDictionary(false);
        Check(ModPacket.Sent.Count == 0, "A client must not publish another player's cooldowns.");
        player.SyncCooldownAddition(false, new CooldownInstance(player.Player, 0, 100, 100));
        player.SyncCooldownRemoval(false, ["one"]);
        player.SyncCooldownDictionary(false);
        Check(ModPacket.Sent.Count == 3, "The owner must still publish cooldown changes.");

        Terraria.Main.netMode = NetmodeID.Server;
        ModPacket.Sent.Clear();
        player.SyncPlayer(4, 2, false);
        Check(ModPacket.Sent.Count == 6, "Join sync must include stats, aim, right click, shields, Exaltation and an owner cooldown request.");
        Check(ModPacket.Sent.Take(5).All(p => p.ToClient == 4 && p.IgnoreClient == 2), "Join sync must preserve routing.");
        var request = ModPacket.Sent.Last();
        Check(request.ToClient == player.Player.whoAmI, "The existing player's cooldown snapshot must be requested from its owner.");
        player.cooldowns["one"] = new(player.Player, 0, 100, 42);
        Terraria.Main.netMode = NetmodeID.MultiplayerClient;
        ModPacket.Sent.Clear();
        Deliver(request, 256);
        var cooldownSnapshot = ModPacket.Sent.Single();
        Terraria.Main.netMode = NetmodeID.Server;
        ModPacket.Sent.Clear();
        Deliver(cooldownSnapshot, 0);
        Check(ModPacket.Sent.Single().ToClient == -1 && ModPacket.Sent.Single().IgnoreClient == 0,
            "The owner's fresh cooldowns must be relayed to the joining peer.");
        Terraria.Main.netMode = NetmodeID.MultiplayerClient;
        player.cooldowns.Clear();
        Deliver(ModPacket.Sent.Single(), 256);
        Check(player.cooldowns["one"].timeLeft == 42, "A late join must receive the owner's current cooldown timer.");

        Terraria.Main.netMode = NetmodeID.SinglePlayer;
        ModPacket.Sent.Clear();
        player.SyncCooldownDictionary(false);
        Check(ModPacket.Sent.Count == 0, "Singleplayer must not send cooldown packets.");
    }

    private static void TestCooldownSnapshots()
    {
        Terraria.Main.netMode = NetmodeID.MultiplayerClient;
        var player = PlayerAt(0);
        player.cooldowns["one"] = new(player.Player, 0, 100, 50);
        player.cooldowns["two"] = new(player.Player, 1, 200, 150);
        ModPacket.Sent.Clear();
        SyncCooldownDictionaryPacket.Send(player, [(2, 300, 250)]);
        var snapshot = ModPacket.Sent.Single();
        using (var reader = Payload(snapshot))
        {
            Check(reader.ReadByte() == 0 && reader.ReadInt32() == 1, "Count must describe the supplied snapshot, not the player's dictionary.");
            Check(reader.BaseStream.Length - reader.BaseStream.Position == 10, "A cooldown entry must occupy exactly ten bytes.");
        }

        Terraria.Main.netMode = NetmodeID.Server;
        ModPacket.Sent.Clear();
        Deliver(snapshot, 1);
        Check(ModPacket.Sent.Count == 0, "A client must not publish another player's cooldown snapshot.");
        ModPacket.Sent.Clear();
        Deliver(snapshot, 0);
        var relay = ModPacket.Sent.Single();
        Check(relay.ToClient == -1 && relay.IgnoreClient == 0, "Cooldown snapshots must reach peers, not echo the owner.");
        Check(relay.Bytes.SequenceEqual(snapshot.Bytes), "Relaying must preserve the complete cooldown snapshot.");

        Terraria.Main.netMode = NetmodeID.MultiplayerClient;
        Deliver(relay, 256);
        Check(player.cooldowns.Count == 1 && player.cooldowns["three"].timeLeft == 250, "Snapshot must cull absent and add new cooldowns.");
        var existing = player.cooldowns["three"];
        ModPacket.Sent.Clear();
        SyncCooldownDictionaryPacket.Send(player, [(2, 400, 200)]);
        Deliver(ModPacket.Sent.Single(), 256);
        Check(ReferenceEquals(existing, player.cooldowns["three"]) && existing.duration == 400 && existing.timeLeft == 200,
            "Updating a cooldown must preserve its handler instance.");
        ModPacket.Sent.Clear();
        SyncCooldownDictionaryPacket.Send(player, []);
        Deliver(ModPacket.Sent.Single(), 256);
        Check(player.cooldowns.Count == 0, "An empty snapshot must remove all local cooldowns.");

        foreach (int count in new[] { -1, 1, int.MaxValue })
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true);
            writer.Write((byte)0);
            writer.Write(count);
            stream.Position = 0;
            using var reader = new BinaryReader(stream);
            Throws<InvalidDataException>(() => SyncCooldownDictionaryPacket.Instance.HandlePacket(reader, 256), "Invalid snapshot count must be rejected before allocation.");
            stream.Position = 0;
            Throws<InvalidDataException>(() => CooldownRemovalPacket.Instance.HandlePacket(reader, 256), "Invalid removal count must be rejected before allocation.");
        }
    }

    private static void TestInvalidCooldownAdditions()
    {
        var player = PlayerAt(0);
        var registeredCooldown = CooldownRegistry.registry[1];
        CooldownRegistry.registry[1] = null;
        try
        {
            foreach (ushort id in new ushort[] { ushort.MaxValue, (ushort)CooldownRegistry.registry.Length, 1 })
            {
                ModPacket.Sent.Clear();
                CooldownAdditionPacket.Send(player, id, 100, 50);
                var addition = ModPacket.Sent.Single();
                foreach (int mode in new[] { NetmodeID.Server, NetmodeID.MultiplayerClient })
                {
                    Terraria.Main.netMode = mode;
                    ModPacket.Sent.Clear();
                    int errors = CalamityMod.CalamityMod.Log.Errors.Count;
                    Deliver(addition, 0);
                    Check(CalamityMod.CalamityMod.Log.Errors.Count == errors + 1,
                        "Unknown cooldown additions must be reported by both servers and clients.");
                    Check(ModPacket.Sent.Count == 0 && player.cooldowns.Count == 0,
                        "Unknown cooldown additions must not be relayed or applied.");
                }
            }
        }
        finally
        {
            CooldownRegistry.registry[1] = registeredCooldown;
        }
    }

    private static void TestRelays()
    {
        Terraria.Main.netMode = NetmodeID.MultiplayerClient;
        var codebreaker = new TECodebreaker { ID = 7, InputtedCellCount = 100, InitialCellCountBeforeDecrypting = 200, HeldSchematicID = 3, ContainsBloodyVein = true, DecryptionCountdown = 600 };
        Terraria.DataStructures.TileEntity.ByID[7] = codebreaker;
        ModPacket.Sent.Clear();
        TEUpdateCodebreakerContainedStuffPacket.Send(codebreaker);
        TEUpdateCodebreakerDecryptCountdownPacket.Send(codebreaker);
        CodebreakerSummonStuffPacket.Send();
        ExoMechSelectionPacket.Send();
        var inputs = ModPacket.Sent.ToArray();
        Terraria.Main.netMode = NetmodeID.Server;
        foreach (var input in inputs)
        {
            ModPacket.Sent.Clear();
            Deliver(input, 0);
            var relay = ModPacket.Sent.Single();
            Check(relay.ToClient == -1 && relay.IgnoreClient == 0 && relay.Bytes.SequenceEqual(input.Bytes), "Codebreaker/Draedon changes must reach all other clients intact.");
        }

        var factory = new TEPowerCellFactory { ID = 8, Time = 9000 };
        Terraria.DataStructures.TileEntity.ByID[8] = factory;
        ModPacket.Sent.Clear();
        TEPowerCellFactoryPacket.Send(factory, 1234, 9999);
        var update = ModPacket.Sent.Single();
        using (var reader = Payload(update))
            Check(reader.BaseStream.Length - reader.BaseStream.Position == 14, "Factory payload must be Int32 ID, Int64 time, Int16 stack.");
        ModPacket.Sent.Clear();
        Deliver(update, 0);
        Check(factory.Time == 9000 && factory.Stack_Internal == 9999, "Client factory edits must not rewind the server clock.");
        using (var reader = Payload(ModPacket.Sent.Single()))
        {
            reader.ReadInt32();
            Check(reader.ReadInt64() == 9000, "Factory relay must carry the authoritative timer.");
        }

        var packet = RageSyncPacket.Instance;
        using var partial = new PartialReadStream([0, 10, 20, 30, 40, 50, 60]);
        using var partialReader = new BinaryReader(partial);
        partial.Position = 3;
        ModPacket.Sent.Clear();
        packet.CloneAndBroadcast(partialReader, 1, 5, 4);
        Check(partial.Position == 3, "Cloning must restore the reader cursor.");
        var clone = ModPacket.Sent.Single();
        Check(clone.ToClient == -1 && clone.IgnoreClient == 4, "Cloning must broadcast excluding the sender.");
        using (var reader = Payload(clone))
            Check(reader.ReadBytes(5).SequenceEqual(new byte[] { 10, 20, 30, 40, 50 }), "Cloning must handle partial stream reads exactly.");
        Throws<InvalidDataException>(() => packet.CloneAndBroadcast(partialReader, 1, 100), "Truncated relay range must be rejected.");
        Throws<InvalidDataException>(() => packet.CloneAndBroadcast(partialReader, -1, 1), "Negative relay range must be rejected.");
    }

    private static void TestNPCSnapshots()
    {
        var npc = Terraria.Main.npc[0];
        npc.active = true;
        npc.type = NPCID.EaterofWorldsHead;
        npc.position = new Vector2(90, 30);
        npc.rotation = 0.5f;
        npc.Calamity().newAI[0] = 1;
        npc.localAI[0] = 2;
        Terraria.Main.netMode = NetmodeID.MultiplayerClient;
        ModPacket.Sent.Clear();
        SyncCalamityNPCAIArrayPacket.Send(npc);
        SyncVanillaNPCLocalAIArrayPacket.Send(npc);
        SyncNPCPosAndRotOnlyPacket.Send(npc);
        Check(ModPacket.Sent.Count == 0, "Clients must not send authoritative NPC snapshots.");

        Terraria.Main.netMode = NetmodeID.Server;
        SyncCalamityNPCAIArrayPacket.Send(npc);
        SyncVanillaNPCLocalAIArrayPacket.Send(npc);
        SyncNPCPosAndRotOnlyPacket.Send(npc);
        var snapshots = ModPacket.Sent.ToArray();
        foreach (var snapshot in snapshots)
        {
            using var reader = Payload(snapshot);
            CalamityPacket handler = snapshot == snapshots[0] ? SyncCalamityNPCAIArrayPacket.Instance :
                snapshot == snapshots[1] ? SyncVanillaNPCLocalAIArrayPacket.Instance : SyncNPCPosAndRotOnlyPacket.Instance;
            npc.Calamity().newAI[0] = npc.localAI[0] = 99;
            npc.position = default;
            handler.HandlePacket(reader, 0);
            Check(npc.Calamity().newAI[0] == 99 && npc.localAI[0] == 99 && npc.position.X == 0, "Server must ignore client-supplied NPC snapshots.");
        }
        Terraria.Main.netMode = NetmodeID.MultiplayerClient;
        foreach (var snapshot in snapshots)
            Deliver(snapshot, 256);
        Check(npc.Calamity().newAI[0] == 1 && npc.localAI[0] == 2 && npc.position.X == 90 && npc.rotation == 0.5f, "Clients must apply server NPC snapshots.");

        foreach (var snapshot in snapshots)
        {
            for (int length = 0; length < snapshot.Bytes.Length; length++)
            {
                Array.Fill(npc.Calamity().newAI, 99f);
                Array.Fill(npc.localAI, 98f);
                npc.position = new Vector2(12, 34);
                npc.rotation = 0.25f;
                int errors = CalamityMod.CalamityMod.Log.Errors.Count;
                using var truncated = new BinaryReader(new MemoryStream(snapshot.Bytes[..length]));
                CalamityNetcode.HandlePacket(CalamityMod.CalamityMod.Instance, truncated, 256);
                Check(CalamityMod.CalamityMod.Log.Errors.Count == errors + 1,
                    "Every truncated authoritative NPC snapshot must report an error.");
                Check(npc.Calamity().newAI.All(value => value == 99f) &&
                    npc.localAI.All(value => value == 98f) &&
                    npc.position.X == 12 && npc.position.Y == 34 && npc.rotation == 0.25f,
                    "Truncated NPC snapshots must not partially apply AI, position or rotation.");
            }
        }
        npc.position = new Vector2(90, 30);

        foreach (var player in Terraria.Main.player)
            player.active = false;
        PlayerAt(0).Player.position = new Vector2(100, 30);
        PlayerAt(1).Player.position = new Vector2(2000, 30);
        PlayerAt(2).Player.position = new Vector2(3000, 30);
        Terraria.Main.netMode = NetmodeID.Server;
        Terraria.Main.GameUpdateCount = 45;
        var sync = new CalamityNetImportantNPC();
        sync.Load();
        sync.SetStaticDefaults();
        ModPacket.Sent.Clear();
        sync.PostAI(npc);
        Check(ModPacket.Sent.Count == 2 && ModPacket.Sent.Select(p => p.ToClient).Order().SequenceEqual(new[] { 1, 2 }),
            "Each distant player must receive one correction; nearby players must receive none.");
        Terraria.Main.GameUpdateCount = 46;
        sync.PostAI(npc);
        Check(ModPacket.Sent.Count == 2, "Long-distance corrections must retain their 45-tick cadence.");
    }

    private static void TestWorldFlags()
    {
        var world = new MiscWorldStateSystem();
        foreach (bool pig in new[] { false, true })
        foreach (bool hog in new[] { false, true })
        {
            CalamityWorld.unlockedTownPig = pig;
            DownedBossSystem.downedHorribleHog = hog;
            CalamityWorld.MoneyStolenByBandit = 123456;
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, true);
            world.NetSend(writer);
            Check(stream.ToArray()[11] == (byte)((pig ? 64 : 0) | (hog ? 32 : 0)), "Pig license and Hog defeat must use separate bits.");
            CalamityWorld.unlockedTownPig = !pig;
            DownedBossSystem.downedHorribleHog = !hog;
            stream.Position = 0;
            using var reader = new BinaryReader(stream);
            world.NetReceive(reader);
            Check(CalamityWorld.unlockedTownPig == pig && DownedBossSystem.downedHorribleHog == hog, "All four Pig/Hog combinations must round-trip independently.");
            Check(stream.Position == stream.Length && CalamityWorld.MoneyStolenByBandit == 123456, "World data must remain aligned.");
        }
    }

    private static void TestPacketDiagnostics()
    {
        var player = PlayerAt(0);
        player.rage = 25;
        ModPacket.Sent.Clear();
        CombatStatsSyncPacket.Send(player);
        var valid = ModPacket.Sent.Single();
        player.rage = 0;
        Terraria.Main.netMode = NetmodeID.Server;
        ModPacket.Sent.Clear();
        PerformanceRecorder.Start("server-test", 4, CalamityNetcode.GetPacketHandlerNames());
        try
        {
            Deliver(valid, 0);
            Deliver(valid, 1);
            using var truncated = new BinaryReader(new MemoryStream(valid.Bytes[..^1]));
            CalamityNetcode.HandlePacket(CalamityMod.CalamityMod.Instance, truncated, 0);
            using var unknown = new BinaryReader(new MemoryStream([255]));
            CalamityNetcode.HandlePacket(CalamityMod.CalamityMod.Instance, unknown, 0);
            var report = PerformanceRecorder.Stop();
            Check(player.rage == 25 && ModPacket.Sent.Count == 1,
                "Enabled diagnostics must preserve valid state/relays and sender ownership.");
            Check(report.Metrics.Single().Metric == "PacketReceive" && report.Metrics.Single().Samples == 4,
                "Each dispatch attempt must record one completed timing scope.");
            var stats = report.ReceivedPackets.Single(packet => packet.Handler == typeof(CombatStatsSyncPacket).FullName);
            Check(stats.Received == 3 && stats.Exceptions == 1,
                "Packet counters must distinguish handler exceptions from logged ownership rejections.");
            Check(report.ReceivedPackets.Single(packet => packet.Handler == "UnknownHandler").Exceptions == 1,
                "Invalid handler IDs must be reported without indexing outside capture metadata.");
        }
        finally
        {
            if (PerformanceRecorder.IsCapturing)
                PerformanceRecorder.Stop();
        }
    }

    private static void TestPacketRegistry()
    {
        using var invalid = new BinaryReader(new MemoryStream([255]));
        int errors = CalamityMod.CalamityMod.Log.Errors.Count;
        CalamityNetcode.HandlePacket(CalamityMod.CalamityMod.Instance, invalid, 0);
        Check(CalamityMod.CalamityMod.Log.Errors.Count == errors + 1, "Unknown packet IDs must log a controlled I/O error.");

        ushort id;
        do { id = CalamityNetcode.RegisterHandler(RageSyncPacket.Instance); } while (id < 255);
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        CalamityNetcode.WriteHandlerNetID(writer, 255);
        Check(stream.Length == 1, "256 handlers must still use byte IDs.");
        CalamityNetcode.RegisterHandler(RageSyncPacket.Instance);
        stream.SetLength(0);
        CalamityNetcode.WriteHandlerNetID(writer, 256);
        Check(stream.Length == 2, "257 handlers must use ushort IDs.");
        stream.Position = 0;
        using var reader = new BinaryReader(stream);
        Check(CalamityNetcode.ReadHandlerNetID(reader) == 256, "Ushort handler IDs must round-trip.");
        Terraria.Main.netMode = NetmodeID.Server;
        Terraria.NetMessage.SentMessageIDs.Clear();
        CalamityNetcode.SyncWorld();
        var system = new CalamityNetcode();
        system.OnModUnload();
        system.PostUpdateEverything();
        Check(Terraria.NetMessage.SentMessageIDs.Count == 0, "Mod unload must discard pending world broadcasts.");
        Check(CalamityNetcode.RegisterHandler(RageSyncPacket.Instance) == 0, "Unloading must leave the registry reusable.");
    }

    private static void TestWorldSyncBatching()
    {
        Terraria.Main.netMode = NetmodeID.Server;
        Terraria.NetMessage.SentMessageIDs.Clear();
        CalamityNetcode.SyncWorld();
        CalamityNetcode.SyncWorld();
        CalamityNetcode.SyncWorld();
        Check(Terraria.NetMessage.SentMessageIDs.Count == 0, "World sync requests should coalesce until the frame ends.");
        new CalamityNetcode().PostUpdateEverything();
        Check(Terraria.NetMessage.SentMessageIDs.SequenceEqual([MessageID.WorldData]), "Multiple same-frame world updates must produce one world-data broadcast.");

        Terraria.Main.netMode = NetmodeID.SinglePlayer;
        CalamityNetcode.SyncWorld();
        new CalamityNetcode().PostUpdateEverything();
        Check(Terraria.NetMessage.SentMessageIDs.Count == 1, "Singleplayer world changes must not enqueue network messages.");

        Terraria.Main.netMode = NetmodeID.Server;
        var system = new CalamityNetcode();
        foreach (Action resetWorld in new Action[] { system.ClearWorld, system.OnWorldUnload })
        {
            Terraria.NetMessage.SentMessageIDs.Clear();
            CalamityNetcode.SyncWorld();
            resetWorld();
            system.PostUpdateEverything();
            Check(Terraria.NetMessage.SentMessageIDs.Count == 0,
                "World clear/unload must discard broadcasts queued for the previous world.");
        }

        Terraria.NetMessage.OnSend = _ => CalamityNetcode.SyncWorld();
        try
        {
            CalamityNetcode.SyncWorld();
            system.PostUpdateEverything();
            Check(Terraria.NetMessage.SentMessageIDs.Count == 1,
                "Requests raised during a flush must not recurse into another broadcast.");
        }
        finally
        {
            Terraria.NetMessage.OnSend = null;
        }
        system.PostUpdateEverything();
        Check(Terraria.NetMessage.SentMessageIDs.Count == 2,
            "A request raised during a flush must survive until the next update.");
        system.PostUpdateEverything();
        Check(Terraria.NetMessage.SentMessageIDs.Count == 2, "Empty updates must not repeat a world broadcast.");
    }

    private sealed class PartialReadStream(byte[] data) : MemoryStream(data)
    {
        public override int Read(Span<byte> buffer) => base.Read(buffer[..Math.Min(buffer.Length, 1)]);
    }
}

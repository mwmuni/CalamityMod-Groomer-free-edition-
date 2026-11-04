using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CalamityMod;
using CalamityMod.Debugging;
using CalamityMod.Enums;
using CalamityMod.Particles;
using CalamityMod.Systems.Graphic;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;

internal static class Program
{
    private static int _assertions;

    private static void Main()
    {
        TestStatistics();
        TestBoundsAndConcurrency();
        TestScopes();
        TestCommandAndHooks();
        TestShaderBuckets();
        TestQueuedParticles();
        Console.WriteLine($"PASS: {_assertions} performance/runtime assertions.");
    }

    private static void Check(bool condition, string message)
    {
        _assertions++;
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private static void Throws<T>(Action action, string message) where T : Exception
    {
        try
        {
            action();
        }
        catch (T)
        {
            _assertions++;
            return;
        }
        throw new InvalidOperationException(message);
    }

    private static void TestStatistics()
    {
        var names = new[] { "KnownPacket" };
        var capture = new PerformanceCapture("test", 100, names);
        names[0] = "ChangedAfterStart";
        for (int i = 100; i > 0; i--)
            capture.Record(PerformanceMetric.GameUpdate, i * Stopwatch.Frequency, 10);
        capture.Record(PerformanceMetric.GameUpdate, 999 * Stopwatch.Frequency, 1000);
        capture.RecordReceivedPacket(0, false);
        capture.RecordReceivedPacket(0, true);
        capture.RecordReceivedPacket(-1, true);
        capture.RecordSentPacket(30);
        capture.RecordSentPacket(18);
        var report = capture.Stop();
        var metric = report.Metrics.Single();
        Check(metric.Samples == 100 && metric.DroppedSamples == 1, "Timing sample storage must be bounded and report drops.");
        Check(metric.P50Milliseconds == 50000 && metric.P95Milliseconds == 95000 &&
            metric.P99Milliseconds == 99000 && metric.MaxMilliseconds == 100000,
            "Percentiles must use the documented nearest-rank definition and millisecond units.");
        Check(metric.AllocatedBytes == 1000, "Allocation totals must cover retained timing samples only.");
        Check(report.ReceivedPackets[0] == new PerformancePacketSummary("KnownPacket", 2, 1),
            "Packet metadata must be snapshotted and exceptions counted separately.");
        Check(report.ReceivedPackets[1] == new PerformancePacketSummary("UnknownHandler", 1, 1),
            "Unknown handler IDs must have an explicit diagnostic category.");
        Check(report.ModPacketSendRequests == 2 && report.ModPacketFramedBytes == 48,
            "Send requests and framed bytes must not imply recipient fan-out.");
        Check(report.SchemaVersion == 1 && report.Mode == "test" && report.SampleLimit == 100 &&
            report.CaptureSeconds >= 0, "Reports must identify their schema and capture bounds.");
        Throws<InvalidOperationException>(() => capture.Stop(), "A capture must not produce mutable repeated snapshots.");
    }

    private static void TestBoundsAndConcurrency()
    {
        Throws<ArgumentOutOfRangeException>(() => new PerformanceCapture("test", 0, []), "Zero sample capacity must fail.");
        Throws<ArgumentOutOfRangeException>(() => new PerformanceCapture("test", PerformanceCapture.MaximumSampleLimit + 1, []),
            "Excessive sample capacity must fail.");
        var capture = new PerformanceCapture("test", 32, ["KnownPacket"]);
        Throws<ArgumentOutOfRangeException>(() => capture.Record(PerformanceMetric.Count, 0, 0), "Invalid metrics must fail.");
        Throws<ArgumentOutOfRangeException>(() => capture.Record(PerformanceMetric.GameUpdate, -1, 0), "Negative timings must fail.");
        Throws<ArgumentOutOfRangeException>(() => capture.Record(PerformanceMetric.GameUpdate, 0, -1), "Negative allocation samples must fail.");

        Parallel.For(0, 1000, _ =>
        {
            capture.Record(PerformanceMetric.PacketReceive, 1, 2);
            capture.RecordReceivedPacket(0, false);
        });
        var report = capture.Stop();
        Check(report.Metrics.Single().Samples == 32 && report.Metrics.Single().DroppedSamples == 968,
            "Concurrent records must preserve the exact sample cap and dropped count.");
        Check(report.ReceivedPackets.Single().Received == 1000, "Concurrent packet totals must not lose increments.");
        capture.RecordReceivedPacket(0, true);
        capture.Record(PerformanceMetric.GameDraw, 1, 1);
        Check(report.ReceivedPackets.Single().Received == 1000 && report.Metrics.Length == 1,
            "Late scopes must not mutate a completed report.");
    }

    private static void TestScopes()
    {
        for (int i = 0; i < 1000; i++)
            PerformanceRecorder.Measure(PerformanceMetric.GameUpdate).Dispose();
        long before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 10000; i++)
            PerformanceRecorder.Measure(PerformanceMetric.GameUpdate).Dispose();
        Check(GC.GetAllocatedBytesForCurrentThread() - before == 0, "Disabled profiling must allocate zero bytes.");

        PerformanceRecorder.Start("test", 2000, ["KnownPacket"]);
        for (int i = 0; i < 1000; i++)
            PerformanceRecorder.Measure(PerformanceMetric.GameUpdate).Dispose();
        before = GC.GetAllocatedBytesForCurrentThread();
        for (int i = 0; i < 1000; i++)
            PerformanceRecorder.Measure(PerformanceMetric.GameUpdate).Dispose();
        Check(GC.GetAllocatedBytesForCurrentThread() - before == 0, "Steady recording must not allocate per sample.");
        Check(PerformanceRecorder.Stop().Metrics.Single().Samples == 2000, "Every completed scope must record once.");

        PerformanceRecorder.Start("test", 1, []);
        var lateScope = PerformanceRecorder.Measure(PerformanceMetric.GameDraw);
        Check(PerformanceRecorder.Stop().Metrics.Length == 0, "In-progress scopes are not completed samples.");
        PerformanceRecorder.Start("next", 1, []);
        lateScope.Dispose();
        Check(PerformanceRecorder.Stop().Metrics.Length == 0, "A late old scope must not enter a new capture.");
        Throws<InvalidOperationException>(() => PerformanceRecorder.Stop(), "Stopping disabled profiling must fail explicitly.");
    }

    private static void TestCommandAndHooks()
    {
        new PerformanceDiagnosticsSystem().Load();
        var command = new PerformanceCommand();
        var caller = new CommandCaller();
        Throws<UsageException>(() => command.Action(caller, "", []), "Missing commands must report usage.");
        foreach (string limit in new[] { "0", "-1", "abc", "36001" })
            Throws<UsageException>(() => command.Action(caller, "", ["start", limit]), "Invalid sample limits must report usage.");
        Check(!PerformanceRecorder.IsCapturing && !On_Main.HasHooks && !Hook.IsActive, "Invalid commands must leave hooks disabled.");
        RunCommand(command, caller, ["stop"]);
        Check(caller.Replies.Last().Contains("No capture is running") && !PerformanceRecorder.IsCapturing,
            "Stopping disabled profiling must notify the caller without changing state.");
        RunCommand(command, caller, ["save"]);
        Check(caller.Replies.Last().Contains("No completed capture"),
            "Saving without a report must notify the caller rather than claiming success.");

        Terraria.Main.netMode = 2;
        var inputThread = new Thread(() => command.Action(caller, "", ["start", "8"])) { IsBackground = true };
        inputThread.Start();
        Check(inputThread.Join(5000), "Submitting a console command must not wait for the game thread.");
        Check(caller.Replies.Last().Contains("runs when a player joins"),
            "Idle dedicated servers must acknowledge queued commands immediately.");
        Check(!PerformanceRecorder.IsCapturing && !On_Main.HasHooks && !Hook.IsActive,
            "Off-thread commands must not install hooks or start captures directly.");
        Terraria.Main.RunMainThreadActions();
        Check(PerformanceRecorder.IsCapturing && On_Main.HasUpdateHook && !On_Main.HasDrawHook && Hook.IsActive,
            "Server captures must install update/send hooks without draw hooks.");
        Check(Hook.CreatedOnThread == Environment.CurrentManagedThreadId, "Detours must be installed on the game thread.");
        Terraria.Netplay.HasClients = true;
        int repliesBeforeTickingCommand = caller.Replies.Count;
        command.Action(caller, "", ["status"]);
        Check(caller.Replies.Count == repliesBeforeTickingCommand,
            "Ticking dedicated servers must not report commands as waiting for players.");
        Terraria.Main.RunMainThreadActions();
        Terraria.Netplay.HasClients = false;
        RunCommand(command, caller, ["start"]);
        Check(caller.Replies.Last().Contains("already running") && PerformanceRecorder.IsCapturing,
            "Duplicate starts must notify the caller without replacing the capture.");
        On_Main.RunUpdate();
        using var ownPacket = new ModPacket(1);
        ownPacket.Write(123);
        ownPacket.Send(-1, 2);
        using var otherPacket = new ModPacket(2);
        otherPacket.Send(4);
        using var unassignedPacket = new ModPacket(-1);
        unassignedPacket.Send();
        Check(ownPacket.Sends == 1 && ownPacket.LastIgnoreClient == 2 && otherPacket.Sends == 1,
            "Profiling must forward own and other mods' sends exactly once without changing routing.");
        Check(unassignedPacket.Sends == 1, "Unassigned/vanilla packets must bypass this mod's profiling.");

        string temporaryDirectory = Path.Combine(Path.GetTempPath(), $"CalamityPerformanceRegression-{Guid.NewGuid():N}");
        Directory.CreateDirectory(temporaryDirectory);
        string blockedDirectory = Path.Combine(temporaryDirectory, "not-a-directory");
        string outputDirectory = Path.Combine(temporaryDirectory, "CalamityPerformance");
        File.WriteAllText(blockedDirectory, "test fixture");
        string originalSavePath = Terraria.Main.SavePath;
        Terraria.Main.SavePath = blockedDirectory;
        try
        {
            int errors = CalamityMod.CalamityMod.Log.Errors.Count;
            RunCommand(command, caller, ["stop"]);
            Check(!PerformanceRecorder.IsCapturing && !On_Main.HasHooks && !Hook.IsActive,
                "Stop must remove all profiling hooks even when exporting fails.");
            Check(CalamityMod.CalamityMod.Log.Errors.Count == errors + 1 &&
                caller.Replies.Last().Contains("Could not write"), "Export failures must log and notify the caller.");
            Check(PerformanceDiagnosticsSystem.LastReport is not null, "Failed exports must retain a retryable report.");

            Terraria.Main.SavePath = temporaryDirectory;
            RunCommand(command, caller, ["save"]);
            string outputPath = Directory.GetFiles(outputDirectory).Single();
            using (var json = JsonDocument.Parse(File.ReadAllText(outputPath)))
            {
                var capture = json.RootElement.GetProperty("capture");
                Check(capture.GetProperty("schemaVersion").GetInt32() == 1 &&
                    capture.GetProperty("mode").GetString() == "Server", "Saved JSON must expose versioned capture metadata.");
                Check(capture.GetProperty("modPacketSendRequests").GetInt64() == 1 &&
                    capture.GetProperty("modPacketFramedBytes").GetInt64() == ownPacket.len,
                    "Only this mod's successful send requests must be counted.");
                Check(capture.GetProperty("metrics").EnumerateArray().Any(metric =>
                    metric.GetProperty("metric").GetString() == "GameUpdate"), "Export must contain measured game-update samples.");
            }
            File.Delete(outputPath);
            Directory.Delete(outputDirectory);

            Terraria.Main.netMode = 1;
            command.Action(caller, "", ["start", "8"]);
            new PerformanceDiagnosticsSystem().OnWorldUnload();
            Terraria.Main.RunMainThreadActions();
            Check(!PerformanceRecorder.IsCapturing && !On_Main.HasHooks && !Hook.IsActive &&
                caller.Replies.Last().Contains("changed"),
                "A queued command must not restart profiling after the world changes.");

            errors = CalamityMod.CalamityMod.Log.Errors.Count;
            Hook.FailNextCreation = true;
            RunCommand(command, caller, ["start"]);
            Check(!PerformanceRecorder.IsCapturing && !On_Main.HasHooks && !Hook.IsActive &&
                CalamityMod.CalamityMod.Log.Errors.Count == errors + 1 &&
                caller.Replies.Last().Contains("See the log"),
                "Detour setup failures must clean up, log the error and notify the caller.");

            RunCommand(command, caller, ["start", "8"]);
            Check(On_Main.HasDrawHook, "Client captures must enable draw timing.");
            On_Main.RunDraw();
            new PerformanceDiagnosticsSystem().OnWorldUnload();
            Check(!PerformanceRecorder.IsCapturing && !On_Main.HasHooks && !Hook.IsActive &&
                PerformanceDiagnosticsSystem.LastReport.Metrics.Single().Metric == "GameDraw",
                "World unload must stop profiling, detach hooks and retain the completed summary.");
            RunCommand(command, caller, ["start", "8"]);
            var unloadThread = new Thread(() => new PerformanceDiagnosticsSystem().Unload()) { IsBackground = true };
            unloadThread.Start();
            Check(unloadThread.Join(5000), "Mod unload must queue cleanup without blocking the loader thread.");
            command.Action(caller, "", ["start", "8"]);
            Terraria.Main.RunMainThreadActions();
            Check(!PerformanceRecorder.IsCapturing && !On_Main.HasHooks && !Hook.IsActive &&
                PerformanceDiagnosticsSystem.LastReport is null && caller.Replies.Last().Contains("changed"),
                "Mod unload must release resources and reject even commands submitted after unload began.");
            Check(Hook.DisposedOnThread == Environment.CurrentManagedThreadId,
                "Mod-unload detours must be disposed on the game thread.");

            new PerformanceDiagnosticsSystem().Load();
            RunCommand(command, caller, ["start", "8"]);
            Check(PerformanceRecorder.IsCapturing && Hook.IsActive, "A fresh mod load must accept capture commands again.");
            new PerformanceDiagnosticsSystem().Unload();
            Terraria.Main.RunMainThreadActions();
        }
        finally
        {
            Terraria.Main.RunMainThreadActions();
            if (PerformanceRecorder.IsCapturing)
                PerformanceDiagnosticsSystem.StopCapture();
            if (Directory.Exists(outputDirectory))
            {
                foreach (string outputPath in Directory.GetFiles(outputDirectory))
                    File.Delete(outputPath);
                Directory.Delete(outputDirectory);
            }
            File.Delete(blockedDirectory);
            Directory.Delete(temporaryDirectory);
            Terraria.Main.SavePath = originalSavePath;
        }
    }

    private static void RunCommand(PerformanceCommand command, CommandCaller caller, string[] arguments)
    {
        command.Action(caller, "", arguments);
        Terraria.Main.RunMainThreadActions();
    }

    private static GeneralParticleHandler SetUpParticles()
    {
        Terraria.Main.netMode = 0;
        GeneralDrawLayerSystem.Reset();
        var handler = new GeneralParticleHandler();
        handler.Load();
        GeneralParticleHandler.particleIDsByTypes[typeof(TestParticle)] = 0;
        return handler;
    }

    private static void TestShaderBuckets()
    {
        var handler = SetUpParticles();
        var firstShader = new Effect();
        var removed = new TestParticle { Shader = firstShader, Additive = true, Lifetime = 1 };
        var sameShader = new TestParticle { Shader = firstShader };
        var laterShader = new TestParticle { Shader = new Effect() };
        GeneralParticleHandler.SpawnParticle(removed);
        GeneralParticleHandler.SpawnParticle(sameShader);
        GeneralParticleHandler.SpawnParticle(laterShader);
        GeneralParticleHandler.Update();
        GeneralDrawLayerSystem.Draw(GeneralDrawLayer.AfterDusts);
        Check(removed.Draws == 0, "Expired particles must not draw.");
        Check(sameShader.Draws == 1 && laterShader.Draws == 1,
            "An empty shader/blend bucket must not suppress later populated buckets.");
        Check(sameShader.ShaderPreparations == 1 && laterShader.ShaderPreparations == 1,
            "Every surviving custom-shader particle must prepare its own shader.");
        Check(!Terraria.Main.spriteBatch.IsDrawing, "Particle drawing must balance every sprite-batch begin/end.");
        handler.OnWorldUnload();
        GeneralDrawLayerSystem.Draw(GeneralDrawLayer.AfterDusts);
        Check(sameShader.Draws == 1 && laterShader.Draws == 1, "World unload must clear particle draw collections.");
    }

    private static void TestQueuedParticles()
    {
        var handler = SetUpParticles();
        var child = new TestParticle();
        var parent = new TestParticle { OnFirstUpdate = () => GeneralParticleHandler.QueueParticleForNextFrame(child) };
        GeneralParticleHandler.SpawnParticle(parent);
        GeneralParticleHandler.Update();
        Check(parent.Time == 1 && child.Time == 0, "Particles queued from update must wait until the next update.");
        GeneralParticleHandler.Update();
        Check(parent.Time == 2 && child.Time == 1, "Queued particles must update exactly once on the following tick.");
        GeneralParticleHandler.RemoveParticle(child);
        GeneralParticleHandler.RemoveParticle(child);
        GeneralParticleHandler.Update();
        int removedAt = child.Time;
        GeneralParticleHandler.Update();
        Check(child.Time == removedAt, "Duplicate kill requests must not leave a particle updating.");
        handler.OnWorldUnload();
    }

    private sealed class TestParticle : Particle
    {
        public Effect Shader;
        public bool Additive;
        public int Draws, ShaderPreparations;
        public Action OnFirstUpdate;
        public override Effect CustomShader => Shader;
        public override bool UseAdditiveBlend => Additive;
        public override bool UseCustomDraw => true;
        public override bool SetLifetime => Lifetime > 0;
        public override void Update()
        {
            if (Time == 1)
                OnFirstUpdate?.Invoke();
        }
        public override void PrepareCustomShader(Effect shader) => ShaderPreparations++;
        public override void CustomDraw(SpriteBatch spriteBatch) => Draws++;
    }
}

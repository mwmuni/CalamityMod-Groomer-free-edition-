#nullable enable
using System;
using System.Reflection;
using System.Threading;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Debugging;

internal sealed class PerformanceDiagnosticsSystem : ModSystem
{
    private static Hook? _sendHook;
    private static bool _drawHooked;
    private static short _modNetId;
    private static int _generation;
    private static bool _unloading;

    internal static PerformanceReport? LastReport { get; private set; }
    internal static int Generation => Volatile.Read(ref _generation);
    internal static bool IsUnloading => Volatile.Read(ref _unloading);

    public override void Load()
    {
        Interlocked.Increment(ref _generation);
        Volatile.Write(ref _unloading, false);
    }

    internal static void StartCapture(int sampleLimit, short modNetId)
    {
        if (PerformanceRecorder.IsCapturing)
            throw new InvalidOperationException("A performance capture is already running.");

        string mode = Main.netMode switch
        {
            NetmodeID.SinglePlayer => "SinglePlayer",
            NetmodeID.MultiplayerClient => "Client",
            NetmodeID.Server => "Server",
            _ => throw new InvalidOperationException("Unknown network mode.")
        };

        try
        {
            _modNetId = modNetId;
            MethodInfo sendMethod = typeof(ModPacket).GetMethod(nameof(ModPacket.Send), [typeof(int), typeof(int)])
                ?? throw new MissingMethodException(typeof(ModPacket).FullName, nameof(ModPacket.Send));
            _sendHook = new Hook(sendMethod, (Action<Action<ModPacket, int, int>, ModPacket, int, int>)MeasurePacketSend);
            On_Main.DoUpdate += MeasureUpdate;
            if (!Main.dedServ)
            {
                On_Main.DoDraw += MeasureDraw;
                _drawHooked = true;
            }

            PerformanceRecorder.Start(mode, sampleLimit, CalamityNetcode.GetPacketHandlerNames());
            LastReport = null;
        }
        finally
        {
            if (!PerformanceRecorder.IsCapturing)
                RemoveHooks();
        }
    }

    internal static PerformanceReport StopCapture()
    {
        try
        {
            LastReport = PerformanceRecorder.Stop();
            return LastReport;
        }
        finally
        {
            RemoveHooks();
        }
    }

    public override void OnWorldUnload()
    {
        Interlocked.Increment(ref _generation);
        if (!PerformanceRecorder.IsCapturing)
            return;

        StopCapture();
        Mod.Logger.Info("Performance capture stopped on world unload. Use calamityperf save to export the retained report.");
    }

    public override void Unload()
    {
        Volatile.Write(ref _unloading, true);
        Interlocked.Increment(ref _generation);
        var logger = Mod.Logger;
        Main.QueueMainThreadAction(() =>
        {
            if (PerformanceRecorder.IsCapturing)
            {
                StopCapture();
                logger.Warn("Performance capture stopped during mod unload. Unsaved capture data will be discarded.");
            }
            else
                RemoveHooks();

            LastReport = null;
        });
    }

    private static void RemoveHooks()
    {
        On_Main.DoUpdate -= MeasureUpdate;
        if (_drawHooked)
        {
            On_Main.DoDraw -= MeasureDraw;
            _drawHooked = false;
        }

        _sendHook?.Dispose();
        _sendHook = null;
    }

    private static void MeasureUpdate(On_Main.orig_DoUpdate orig, Main self, ref GameTime gameTime)
    {
        using var measurement = PerformanceRecorder.Measure(PerformanceMetric.GameUpdate);
        orig(self, ref gameTime);
    }

    private static void MeasureDraw(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
    {
        using var measurement = PerformanceRecorder.Measure(PerformanceMetric.GameDraw);
        orig(self, gameTime);
    }

    private static void MeasurePacketSend(Action<ModPacket, int, int> orig, ModPacket packet, int toClient, int ignoreClient)
    {
        if (packet.netID < 0 || packet.netID != _modNetId)
        {
            orig(packet, toClient, ignoreClient);
            return;
        }

        using var measurement = PerformanceRecorder.Measure(PerformanceMetric.PacketSend);
        orig(packet, toClient, ignoreClient);
        measurement.RecordSentPacket(packet.len);
    }
}

using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Debugging;

public sealed class PerformanceCommand : ModCommand
{
    private static readonly JsonSerializerOptions ReportOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public override string Command => "calamityperf";
    public override CommandType Type => CommandType.Chat | CommandType.Console;
    public override string Usage => "/calamityperf start [samples-per-metric] | stop | save | status";
    public override string Description => "Capture bounded local timing/allocation/network diagnostics without changing gameplay settings.";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length == 0)
            throw new UsageException(Usage);

        string operation = args[0];
        int limit = PerformanceCapture.DefaultSampleLimit;
        switch (operation)
        {
            case "start" when args.Length <= 2:
                if (args.Length == 2 && (!int.TryParse(args[1], NumberStyles.None, CultureInfo.InvariantCulture, out limit) ||
                    limit < 1 || limit > PerformanceCapture.MaximumSampleLimit))
                    throw new UsageException($"Sample limit must be between 1 and {PerformanceCapture.MaximumSampleLimit}.");
                break;
            case "stop" or "save" or "status" when args.Length == 1:
                break;
            default:
                throw new UsageException(Usage);
        }

        int generation = PerformanceDiagnosticsSystem.Generation;
        Main.QueueMainThreadAction(() =>
        {
            if (PerformanceDiagnosticsSystem.IsUnloading || generation != PerformanceDiagnosticsSystem.Generation)
            {
                caller.Reply("The world or mod changed before the performance command ran. Please retry.", Color.OrangeRed);
                return;
            }

            try
            {
                Execute(caller, operation, limit);
            }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or
                MemberAccessException or TypeLoadException or NotSupportedException)
            {
                Mod.Logger.Error("Could not execute the local performance command.", exception);
                caller.Reply("Could not execute the performance command. See the log for details.", Color.OrangeRed);
            }
        });

        // Dedicated servers only drain queued main-thread actions from Update, which is skipped while nobody is connected.
        if (Main.dedServ && !Netplay.HasClients)
            caller.Reply("Queued: the dedicated server pauses updates while no players are connected, so this runs when a player joins.");
    }

    private void Execute(CommandCaller caller, string operation, int limit)
    {
        switch (operation)
        {
            case "start":
                if (PerformanceRecorder.IsCapturing)
                {
                    caller.Reply("A capture is already running. Stop it before starting another.", Color.OrangeRed);
                    break;
                }

                PerformanceDiagnosticsSystem.StartCapture(limit, Mod.NetID);
                caller.Reply($"Local capture started (first {limit} samples per metric). Use calamityperf stop to stop and save.");
                break;
            case "stop":
                if (!PerformanceRecorder.IsCapturing)
                {
                    caller.Reply("No capture is running.", Color.OrangeRed);
                    break;
                }
                PerformanceDiagnosticsSystem.StopCapture();
                SaveReport(caller);
                break;
            case "save":
                SaveReport(caller);
                break;
            case "status":
                caller.Reply(PerformanceRecorder.IsCapturing
                    ? "Local capture is running. Timings are capped; packet counters continue until stop."
                    : "Local capture is disabled.");
                break;
            default:
                throw new ArgumentException("Unknown performance command.", nameof(operation));
        }
    }

    private void SaveReport(CommandCaller caller)
    {
        var report = PerformanceDiagnosticsSystem.LastReport;
        if (report is null)
        {
            caller.Reply("No completed capture is available. Start and stop a capture first.", Color.OrangeRed);
            return;
        }

        string directory = Path.Combine(Main.SavePath, "CalamityPerformance");
        string path = Path.Combine(directory, $"capture-{DateTime.UtcNow:yyyyMMdd-HHmmss-fffffff}-{report.Mode}.json");
        try
        {
            Directory.CreateDirectory(directory);
            using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Read);
            JsonSerializer.Serialize(stream, new
            {
                ModVersion = Mod.Version.ToString(),
                BuildId = typeof(PerformanceCommand).Assembly.ManifestModule.ModuleVersionId,
                Runtime = RuntimeInformation.FrameworkDescription,
                Capture = report
            }, ReportOptions);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Mod.Logger.Error("Could not export the local performance capture.", exception);
            caller.Reply("Could not write the capture. See the log; the report is retained for calamityperf save.", Color.OrangeRed);
            return;
        }

        caller.Reply($"Saved local performance capture: {path}");
    }
}

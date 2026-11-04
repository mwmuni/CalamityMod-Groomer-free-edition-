#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace CalamityMod.Debugging;

internal enum PerformanceMetric
{
    GameUpdate,
    GameDraw,
    ParticleUpdate,
    ParticleDraw,
    PacketReceive,
    PacketSend,
    Count
}

internal sealed record PerformanceMetricSummary(
    string Metric, int Samples, long DroppedSamples,
    double P50Milliseconds, double P95Milliseconds, double P99Milliseconds,
    double MaxMilliseconds, long AllocatedBytes);

internal sealed record PerformancePacketSummary(string Handler, long Received, long Exceptions);

internal sealed record PerformanceReport(
    int SchemaVersion, string Mode, int SampleLimit, double CaptureSeconds,
    PerformanceMetricSummary[] Metrics, PerformancePacketSummary[] ReceivedPackets,
    long ModPacketSendRequests, long ModPacketFramedBytes);

internal sealed class PerformanceCapture
{
    internal const int DefaultSampleLimit = 3600;
    internal const int MaximumSampleLimit = 36000;

    private readonly object _gate = new();
    private readonly string _mode;
    private readonly int _sampleLimit;
    private readonly long _startedAt;
    private readonly long[][] _elapsedTicks;
    private readonly int[] _sampleCounts = new int[(int)PerformanceMetric.Count];
    private readonly long[] _droppedSamples = new long[(int)PerformanceMetric.Count];
    private readonly long[] _allocatedBytes = new long[(int)PerformanceMetric.Count];
    private readonly string[] _packetNames;
    private readonly long[] _receivedPackets;
    private readonly long[] _packetExceptions;
    private long _sendRequests;
    private long _framedBytes;
    private bool _stopped;

    internal PerformanceCapture(string mode, int sampleLimit, string[] packetNames)
    {
        ArgumentNullException.ThrowIfNull(mode);
        ArgumentNullException.ThrowIfNull(packetNames);
        ArgumentOutOfRangeException.ThrowIfLessThan(sampleLimit, 1);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(sampleLimit, MaximumSampleLimit);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(packetNames.Length, ushort.MaxValue + 1);

        _mode = mode;
        _sampleLimit = sampleLimit;
        _packetNames = (string[])packetNames.Clone();
        _receivedPackets = new long[packetNames.Length + 1];
        _packetExceptions = new long[packetNames.Length + 1];
        _elapsedTicks = new long[(int)PerformanceMetric.Count][];
        for (int i = 0; i < _elapsedTicks.Length; i++)
            _elapsedTicks[i] = new long[sampleLimit];
        _startedAt = Stopwatch.GetTimestamp();
    }

    internal void Record(PerformanceMetric metric, long elapsedTicks, long allocatedBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(elapsedTicks);
        ArgumentOutOfRangeException.ThrowIfNegative(allocatedBytes);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((uint)metric, (uint)PerformanceMetric.Count);
        lock (_gate)
        {
            if (_stopped)
                return;

            int index = (int)metric;
            int count = _sampleCounts[index];
            if (count == _sampleLimit)
            {
                _droppedSamples[index]++;
                return;
            }

            _elapsedTicks[index][count] = elapsedTicks;
            _sampleCounts[index]++;
            _allocatedBytes[index] += allocatedBytes;
        }
    }

    internal void RecordReceivedPacket(int handlerId, bool threwException)
    {
        lock (_gate)
        {
            if (_stopped)
                return;

            int index = (uint)handlerId < (uint)_packetNames.Length ? handlerId : _packetNames.Length;
            _receivedPackets[index]++;
            if (threwException)
                _packetExceptions[index]++;
        }
    }

    internal void RecordSentPacket(int framedBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(framedBytes);
        lock (_gate)
        {
            if (_stopped)
                return;

            _sendRequests++;
            _framedBytes += framedBytes;
        }
    }

    internal PerformanceReport Stop()
    {
        lock (_gate)
        {
            if (_stopped)
                throw new InvalidOperationException("The performance capture has already stopped.");

            _stopped = true;
            double seconds = (Stopwatch.GetTimestamp() - _startedAt) / (double)Stopwatch.Frequency;
            List<PerformanceMetricSummary> metrics = [];
            for (int i = 0; i < _elapsedTicks.Length; i++)
            {
                int count = _sampleCounts[i];
                if (count == 0)
                    continue;

                Array.Sort(_elapsedTicks[i], 0, count);
                metrics.Add(new(
                    ((PerformanceMetric)i).ToString(), count, _droppedSamples[i],
                    Percentile(i, count, 0.50), Percentile(i, count, 0.95),
                    Percentile(i, count, 0.99), Percentile(i, count, 1),
                    _allocatedBytes[i]));
            }

            List<PerformancePacketSummary> packets = [];
            for (int i = 0; i < _receivedPackets.Length; i++)
            {
                if (_receivedPackets[i] != 0)
                    packets.Add(new(
                        i < _packetNames.Length ? _packetNames[i] : "UnknownHandler",
                        _receivedPackets[i], _packetExceptions[i]));
            }

            return new(1, _mode, _sampleLimit, seconds, [.. metrics], [.. packets], _sendRequests, _framedBytes);
        }
    }

    private double Percentile(int metric, int count, double fraction) =>
        _elapsedTicks[metric][(int)Math.Ceiling(count * fraction) - 1] * 1000d / Stopwatch.Frequency;
}

internal static class PerformanceRecorder
{
    private static PerformanceCapture? _current;

    internal static bool IsCapturing => Volatile.Read(ref _current) is not null;

    internal static void Start(string mode, int sampleLimit, string[] packetNames)
    {
        var capture = new PerformanceCapture(mode, sampleLimit, packetNames);
        if (Interlocked.CompareExchange(ref _current, capture, null) is not null)
            throw new InvalidOperationException("A performance capture is already running.");
    }

    internal static PerformanceReport Stop()
    {
        var capture = Interlocked.Exchange(ref _current, null)
            ?? throw new InvalidOperationException("No performance capture is running.");
        return capture.Stop();
    }

    internal static PerformanceScope Measure(PerformanceMetric metric) => new(Volatile.Read(ref _current), metric);

}

internal readonly ref struct PerformanceScope
{
    private readonly PerformanceCapture? _capture;
    private readonly PerformanceMetric _metric;
    private readonly long _startedAt;
    private readonly long _allocatedAtStart;

    internal PerformanceScope(PerformanceCapture? capture, PerformanceMetric metric)
    {
        _capture = capture;
        _metric = metric;
        _startedAt = capture is null ? 0 : Stopwatch.GetTimestamp();
        _allocatedAtStart = capture is null ? 0 : GC.GetAllocatedBytesForCurrentThread();
    }

    internal void RecordReceivedPacket(int handlerId, bool threwException) =>
        _capture?.RecordReceivedPacket(handlerId, threwException);

    internal void RecordSentPacket(int framedBytes) => _capture?.RecordSentPacket(framedBytes);

    public void Dispose()
    {
        if (_capture is null)
            return;

        long elapsedTicks = Stopwatch.GetTimestamp() - _startedAt;
        long allocatedBytes = GC.GetAllocatedBytesForCurrentThread() - _allocatedAtStart;
        _capture.Record(_metric, elapsedTicks, allocatedBytes);
    }
}

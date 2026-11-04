# Calamity Mod
[![Discord](https://img.shields.io/discord/225030931008847874)](https://discord.gg/calamity)
- [Official site](https://calamitymod.com/)
- [Steam Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=2824688072)
- [Discord](https://discord.gg/calamity)
- [Official Wiki](https://calamitymod.wiki.gg/)
- [Patreon](https://patreon.com/CalamityMod)

The Calamity Mod is a vast content mod for Terraria which adds dozens of bosses and thousands of items to create an intense and varied experience. The mod must be installed using [tModLoader](https://github.com/tModLoader/tModLoader).

The Calamity Mod and all associated source code, visual assets, audio assets, characters, names, logos, trademarks, branding, and related materials are proprietary to Azafure, LLC, a Virginia limited liability company (*"Azafure"*), and are protected by applicable intellectual property laws. All rights reserved. Except for the limited permission necessary to access and contribute to the repository in accordance with Azafure's License terms (*see* `LICENSE.md` *in the License tab above*), nothing in this Page or the License terms grants any license, permission, or ownership interest in or to Azafure's intellectual property.

**Note:** This repository contains only the latest public release of the Calamity Mod and any public contributions made since then.

Active development by Calamity developers occurs on a separate, private repository. Contributions made here will be cherry picked into that repository. When an update is released to the Steam Workshop, the `1.4.4-release` branch is replaced with the matching branch from the private repository.

Issues are disabled on this repository. To report a bug, please join the [Calamity Discord](https://discord.gg/calamity) and use the `#bug-reports-read-pins` forum therein.

Please do not submit resprites via pull requests. Instead, go to the [Calamity Art Discord](https://discord.gg/CKByxd5) and use the Asset Submission system if you want to propose a resprite for Calamity.

## Networking validation

Run `dotnet run --project Tests\NetworkingRegression\NetworkingRegression.csproj` to exercise packet serialization, server relays, idle/active player traffic, join synchronization, distant NPC routing, world-sync coalescing/lifecycle, malformed cooldown IDs, truncated NPC snapshots, and world-flag round trips. This dependency-free harness compiles the networking sources against lightweight Terraria doubles; it does not replace in-game multiplayer testing.

Idle combat stats use one atomic 120-tick heartbeat; changed or active stats retain the 15-tick correction interval, with the three fields combined into one packet (95.8% fewer idle stat packets and about 89% fewer idle stat bytes than the original three-per-check baseline). World-data broadcasts requested during the same server tick are coalesced into one send. Joining players receive existing player state, with fresh cooldown snapshots requested from their owners. NPC AI and position snapshots originate only on the server. All peers must run the same mod build.

Before release, test with a server and at least two clients: join during a boss fight, check distant worm segments, modify a codebreaker from either client, and verify factory stacks, shields, cooldown expiration, and town Pig unlocks on both clients.

## Local performance captures

Profiling is disabled by default and does not change synchronization rates, simulation, or graphics settings. Capture controls and cleanup run on the game thread; queued commands are cancelled if the world or mod changes before execution. A dedicated server does not update while no players are connected, so console commands issued then are acknowledged as queued and run when a player joins; start captures after players connect. Use these commands in local chat, or without the leading slash in a dedicated-server console:

- `/calamityperf start 3600`: record the first 3,600 samples per metric (allowed range: 1-36,000).
- `/calamityperf status`: check whether recording is enabled.
- `/calamityperf stop`: stop, detach timing/send hooks, and save a local JSON report.
- `/calamityperf save`: export the last completed report again, including after a write failure.

Reports are stored in `CalamityPerformance` under tModLoader's save directory. They include the mod build ID, runtime, process role, p50/p95/p99/max scope durations, sampled current-thread allocations, incoming Calamity handler invocation/exception counts, and outgoing Calamity mod-packet send-request/framed-byte totals. They do not contain packet contents, player identifiers, or network addresses.

Timing storage is bounded. Once a metric reaches its limit, later timing samples are counted as dropped; packet counters continue until stopped. Allocation totals correspond only to retained timing samples. Empty metrics are omitted. World unload stops recording and retains the summary for `save`; mod unload discards unsaved data. Start/stop/export commands should be performed outside the workload being compared.

Interpret the measurements carefully:

- Timings are inclusive synchronous wall-clock durations, including waits and pauses, not CPU execution time, GPU timings or presentation intervals. Nested scopes overlap: do not sum their percentiles or allocation totals. Particle draw samples are per draw-layer invocation, not per complete frame.
- Send totals count completed API requests and their framed buffer lengths, not successful recipient deliveries or wire bytes. Broadcast fan-out, retransmissions, Calamity-triggered vanilla packets and extra-AI payloads are not separately attributed.
- Incoming exception counts cover exceptions observed by the dispatcher, not every ownership/direction rejection logged by a handler.
- Warm up first, compare identical worlds/equipment/settings and builds on the same hardware, and repeat captures. Record resolution, zoom, lighting, frame settings and player counts alongside reports. Use an external profiler for GPU, process-wide allocation/GC, and complete frame-presentation evidence.

Run `dotnet run --project Tests\PerformanceRegression\PerformanceRegression.csproj --configuration Release` for bounded-buffer/percentile/concurrency checks, zero-allocation scope checks, command/export/hook-lifecycle tests, and particle draw/queue regressions. This harness links production code to rendering/hook doubles; it does not exercise a GPU, real detour installation, or live multiplayer. Both harnesses must pass alongside `dotnet build CalamityMod.csproj`; a dedicated server with two real clients remains the release gate.

For live validation, install the dependencies declared in [build.txt](build.txt) and verify the server log confirms that Calamity loaded and the console recognizes `calamityperf status`. tModLoader can disable a failed mod and still start a listening server; that is not a valid Calamity test. tModLoader uses the source folder name as the internal mod name, so build from a folder named `CalamityMod`; otherwise loading fails with `Mod name ... does not match assembly name CalamityMod`. When scripting a Windows dedicated-server console, write stdin as UTF-16LE because tModLoader reads console input as Unicode. The regressions verify logic and allocations; instrumentation-overhead comparisons still require repeated in-game captures.

**Do not redistribute the Calamity Mod.** Creating your own builds of the mod from an in-development sourcetree may cause confusion in the community and make players' mods fail to sync in multiplayer. Azafure will remove any unauthorized copies of the Calamity Mod published to the Steam Workshop or rehosted elsewhere, and reserves all rights and remedies available to Azafure.
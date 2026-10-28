# Copilot instructions for Calamity Mod

## Overview

This repository contains the public source tree for the Calamity Mod, a large Terraria content mod built with tModLoader. It is not a typical application; gameplay content is spread across many domain folders (`Items`, `NPCs`, `Projectiles`, `Buffs`, `Tiles`, `World`, `UI`, `Systems`, `Localization`, etc.) with a central mod bootstrap at the repository root.

The important entry points are:

- `CalamityMod.cs` — root mod class; handles load/unload hooks, render-target setup, music integration, mod calls, and packet handling.
- `CalamityConfig.cs` — client-side `ModConfig` for graphics and UI settings.
- `CalamityKeybinds.cs` — `ModSystem`-based keybind registration and accessory hotkey management.
- `CalamityNetcode.cs` — multiplayer packet handling.
- `Balancing/` — large, rules-driven balance adjustments for NPCs and classes.

This repo is a public mirror of a release branch from a private development pipeline; do not assume a full upstream issue tracker or private tooling is available here.

## Build, lint, and validation

This repository uses tModLoader and the project imports `..\tModLoader.targets` from a sibling `tmod` checkout. The repository's CI workflow is the best reference for local validation:

- Restore dependencies:
  - `dotnet restore`
- Validate formatting/style:
  - `dotnet format style CalamityMod.csproj --verify-no-changes`
- Compile check when a proper tModLoader environment is available:
  - `dotnet build CalamityMod.csproj`

The GitHub Actions workflow in `.github/workflows/lint.yml` bootstraps a temporary `tModLoader.targets` import and a temporary tModLoader checkout before running the formatter. If you need to reproduce CI locally, follow that same pattern rather than assuming a standard .NET project setup.

There is no checked-in automated test suite in this repository. There is no `dotnet test` command to run here, and there are no project-level tests to target individually.

## High-level architecture

The overall architecture is a domain-oriented content mod, not a layered MVC or service-driven app:

- The root `CalamityMod` class is the mod entry point and holds global setup.
- Most gameplay logic is implemented through `ModSystem`, `GlobalNPC`, `GlobalItem`, `GlobalProjectile`, and other tModLoader extension points. Behavior is often distributed across the relevant content folder instead of centralized in one giant service layer.
- `Balancing/` contains curated game-balance rules, often arranged as condition-based balancing changes keyed by NPC, skill class, or projectile type.
- `Systems/`, `MainMenu/`, `UI/`, and `Graphics/` contain shared cross-cutting systems and rendering hooks.
- `Localization/` stores language data for UI and content text.
- `Backgrounds/` and related visual folders hold custom world backgrounds and render assets.

Most new work should be added in the matching content area and use the existing ModLoader extension patterns rather than introducing a new framework or architecture.

## Key conventions

- Follow the repository's namespace and folder layout. New types generally belong in the domain folder that matches their content (`Items`, `NPCs`, `Projectiles`, `Systems`, `UI`, etc.).
- Prefer tModLoader patterns already in use in the codebase: `ModSystem`, `ModConfig`, `GlobalItem`, `GlobalNPC`, `GlobalProjectile`, `ModItem`, `ModNPC`, `ModProjectile`, and `ModBuff`.
- When work touches graphics or render setup, use `Main.QueueMainThreadAction(...)` for main-thread-only initialization. The code explicitly does this because loading is multithreaded.
- Keep static initialization and early setup in the correct mod lifecycle (`Load`, `Unload`, `SetStaticDefaults`, or similar) instead of building ad hoc startup code outside the tModLoader lifecycle.
- Respect the existing balance and content organization: a feature often spans several files, and cross-cutting changes should be kept aligned with the matching domain rather than hidden in a generic utility class.
- Keep changes surgical and repository-native. This mod is extremely large; it is easiest to reason about if you stay within the same content family and match the local naming conventions.

## Working style reminders

- This repo is a large public source tree with many generated and asset-heavy files. Prefer narrow, file-specific edits over broad refactors.
- If you need to validate a change, prefer the repo's existing lint/format workflow and the minimal compile signal that matches a tModLoader environment.
- Do not assume issue tracking or PR automation is available in this checkout; the public repo is intentionally minimal.

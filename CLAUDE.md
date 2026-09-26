# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

IdleKnight (in-code/asset references still say "Earl" / "IdleEarl" — this is the NewGrounds game "Earl", second release: https://www.newgrounds.com/portal/view/999110) is a Unity 2D idle/incremental game built with Unity 6000.0.45f1. Gameplay is arena-based combat with passive income: players auto-fight waves of enemies in 30-second arena rounds, earn gold, buy upgrades, and periodically "ascend" for prestige currency (monster credits, diamonds).

The game ships to WebGL and is hosted embedded on a portal (Newgrounds); the WebGL/browser bridge is load-bearing (see Save System below), not incidental.

## Development Commands

- Open the project with Unity Editor **6000.0.45f1** (pinned in `ProjectSettings/ProjectVersion.txt`).
- Edit C# in Rider or Visual Studio via `IdleKnight.sln`.
- Scripted desktop build:
  ```bash
  %UNITY_PATH% -projectPath "C:\Repo\IdleKnight" -quit -batchmode -executeMethod BuildPipeline.BuildPlayer
  ```
  Output goes to `Build/`. Production builds are WebGL; use the Editor's Build Settings for WebGL builds since there's no scripted WebGL build command here.
- No automated test suite exists. Validation is manual play-mode testing in the Editor. If adding tests, put EditMode specs in `Assets/Tests/EditMode` and PlayMode specs in `Assets/Tests/PlayMode`, named after the feature under test.

## Code Architecture

Most gameplay scripts live directly under `Assets/Script/`, organized by feature:

- **Managers/** — `GameManager.cs` (~1700 lines) is the central controller: arena round flow/timers, spawning trigger, gold/HP bar updates, save/load orchestration, PlayFab stat pushes, and cheat-key debug hooks. `AudioManager`, `CacheManager`, `MusicManagerScript` round out cross-cutting systems.
- **Actors/** — `PlayerScript`, `ActorBase`, and enemy AI in `Actors/Enemies/` (`IActorController` implementations like `ActorChasePlayer`, `ActorDefaultWalker`, boss shooters). `Actors/Spawning/` handles enemy/chest spawning and pooling (`ActorCache`, `SpawnUtil`, `FormationUtil`).
- **Weapons/** — `WeaponBase` plus per-weapon subclasses (dagger throws, chain zaps/`Zapper`, grenades, sawblade, staff, etc.); `Weapons/Custom/` holds special-case weapon behavior.
- **Upgrades/** — `UpgradeManager` drives the upgrade shop UI; each purchasable upgrade has its own manager under `Upgrades/Managers/` (e.g. `KnifeDamageManager`, `FastFeetManager`, `GoldPerRoundManager`). **See `Assets/Script/Upgrades/Managers/readme.txt` for the required steps to add a new upgrade** (prefab, manager script, wiring into `UpgradeManager`, button events).
- **Play/** — arena/canvas plumbing: `GameCanvasScript`, `ProjectileManager`/`ProjectileCache` (pooled projectiles), `GameModeData`, kill/reset interfaces (`IKillableObject`, `IKillOnSaveWipe`, `IOnStartGameKillableObject`).
- **Data/** — ScriptableObject-backed data (`EnemyPrefabs`, `SpriteData`, `AudioData`, `SkinsAnimationList`).
- **Achievements/**, **Bling/** (floating text, pickups, explosions) — self-contained feature folders.
- **Misc/** — grab-bag of core utilities: `SaveGame`/`SaveGameAscend` (persistence), `Decimal512` (in `Decimal256.cs` — filename predates a rename, the type is `Decimal512`) with matching `Format256`/`Format64`/`FormatScientific` formatters for huge idle-game numbers, `GameObjectPool` (generic pooling used throughout instead of `Instantiate`/`Destroy`), `PlayerUpgrades` (aggregated upgrade effect totals), `Obfuscation` (save data), `RndUtil`, `PositionUtility`.
- Root-level scripts directly in `Assets/` (not `Assets/Script/`) include scene-specific glue (`TitleButtonScript`, `AscendUpgradeCardScript`, `SkinsPopupScript`, popups) and `JsMappings.cs`, the WebGL JS interop layer.

### Global singletons

- `G` (`Assets/Script/G.cs`) — global access point: `G.D` singleton instance, player transform/position cache, UI colors (pre-baked to hex for rich-text), input helpers (`MoveUpTap`, `SelectionTap`, etc.), and cheat-key checks.
- `GameManager.Instance` — arena/round state, `GameTime`/`GameDeltaTime`, debug output (`GameManager.SetDebugOutput` via `G.Dbg`).

### Save system

`SaveGame` (`Assets/Script/Misc/SaveGame.cs`) persists `SaveGameMembers` as obfuscated JSON. On WebGL it writes to **both** browser `localStorage` (via `JsMappings`, which survives page updates less reliably) and Unity `PlayerPrefs` (survives localStorage clears but can get wiped on game updates) — on load it reconciles both, preferring the more recent one. Non-WebGL platforms use `PlayerPrefs` only. Auto-save runs every 5 seconds (`GameManager.AutoSaveInterval`); PlayFab stats are pushed every 10 minutes (`GameManager.SendStatsInterval`).

`SaveGameMembers` fields are split into permanent (survive ascend) vs. per-run state — some fields carry date suffixes (e.g. `MonsterCredits_09_08_2025`) marking a past value migration; don't strip the suffix without checking `SaveGameAscend` stays in sync (there's a `// SaveGameAscend must be kept up to date with this` note at the top of the file).

### Combat & Arena

- Arena rounds are 30 seconds (`GameManager.RoundTimeSeconds`); enemy composition scales with `SaveGame.Members.ArenaLevel`.
- Damage/pickups flow through pooled objects (`GameObjectPool`, `ProjectileCache`) rather than per-hit `Instantiate`/`Destroy`.

## Coding Conventions

- 4-space indentation; PascalCase for public members, camelCase for private fields.
- Serialized fields declared at the top of each `MonoBehaviour` for Inspector organization.
- Avoid introducing new namespaces unless a feature genuinely spans multiple folders.
- Minimal commenting — prefer self-documenting code; existing comments in this codebase are often dated notes-to-self about migrations/gotchas (e.g. in `SaveGame.cs`) rather than API docs — read them, they carry real constraints.
- Use `const` fields for game-balance values (see `GameManager.RoundTimeSeconds`, `AutoSaveInterval`, `SendStatsInterval`).

## Configuration & Secrets

- PlayFab credentials are configured through the PlayFab editor extension (`Assets/PlayFabEditorExtensions`), not hardcoded.
- Secrets/local settings live in `UserSettings/` — never commit this directory (already gitignored).
- Never commit `Library/`, `Temp/`, `Logs/`, or credential files.

## Performance Notes

- Target frame rate is pinned to 60 (`Application.targetFrameRate = 60` in `GameManager`).
- Passive income is calculated per-frame with realtime-delta clamping (see `GameManager.GameDeltaTime` usage) to stay stable across frame-rate hitches and WebGL tab-throttling.
- Large numbers (gold, monster credits, damage) use `Decimal512`, not built-in numeric types — use it (and the matching `Format*` helpers) for any new balance-affecting value that can grow unbounded.

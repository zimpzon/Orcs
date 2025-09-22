# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

IdleEarl2 is a Unity 2D idle/incremental game built with Unity 6000.0.45f1. The game features arena-based combat with passive income mechanics, upgrades, and progression systems. Players fight through increasingly difficult arenas while earning gold and upgrading their abilities.

## Development Commands

### Opening the Project
- Open with Unity Editor version 6000.0.45f1 (as specified in `ProjectSettings/ProjectVersion.txt`)
- Use `IdleEarl2.sln` for C# development in Rider or Visual Studio

### Building
For automated desktop builds:
```bash
%UNITY_PATH% -projectPath "C:\dev\IdleEarl2" -quit -batchmode -executeMethod BuildPipeline.BuildPlayer
```
Output goes to `Build/` directory.

### Dependencies
- Restore dependencies via `Packages/manifest.json` (Unity handles this automatically)
- Key packages include: Unity Ads, Timeline, UGUI, Test Framework, Purchasing

## Code Architecture

### Core Structure
- **Assets/**: All editable content
  - **Script/**: Main code organization by feature folders
    - **Managers/**: Core game management (GameManager, AudioManager, etc.)
    - **Actors/**: Player and enemy entities
    - **Weapons/**: Combat system components
    - **Upgrades/**: Progression and enhancement systems
    - **Play/**: Arena gameplay mechanics
    - **Data/**: Data structures and ScriptableObjects
  - **ScriptableObject/**: Shared data assets
  - **Prefab/**: UI and game object prefabs
  - **Texture/**, **Audio/**, **Font/**: Art and media assets

### Key Systems

#### Game Management
- `GameManager` (`Assets/Script/Managers/GameManager.cs`): Central game controller handling arena rounds, combat flow, save/load, and core game loop
- `G` (`Assets/Script/G.cs`): Global singleton for shared data, player references, input handling, and utility functions
- `SaveGame`: Persistent data management with both local storage and PlayerPrefs fallback

#### Combat & Arena
- Arena-based rounds with 30-second time limits
- Enemy spawning scales with arena level
- HP bar system tracks total enemy health
- Damage sources: dagger throws, chain zaps, summoned entities
- Auto-pickup system for gold and experience

#### Progression Systems
- Multiple upgrade trees (damage, passive income, special abilities)
- Ascension system for prestige mechanics
- Monster credits and diamond currency
- Skin/cosmetic unlocks
- Buy amount selection (1, 10, 100, or "next x2 bonus")

#### Audio & Visual Effects
- Particle systems for combat feedback
- Camera shake on hits
- Floating damage/gold numbers
- LeanTween for UI animations

## Coding Conventions

- **Style**: 4-space indentation, PascalCase for public members, camelCase for private fields
- **MonoBehaviour Structure**: Serialized fields at top for Inspector organization
- **Namespaces**: Avoid new namespaces unless spanning multiple folders
- **Comments**: Minimal commenting - prefer self-documenting code
- **Magic Numbers**: Use const fields for game balance values

## Testing & Validation

- Primarily manual testing (no automated test suites included)
- Place any new tests in `Assets/Tests/EditMode` or `Assets/Tests/PlayMode`
- Include reproduction steps and screenshots for defects

## Configuration & Secrets

- PlayFab credentials configured through PlayFab editor extension
- Store secrets in `UserSettings/` (excluded from version control)
- Music/SFX volume controlled via SaveGame members
- Debug features available via cheat key combinations (Ctrl+key combinations)

## Important Files to Never Commit

- `Library/` - Unity-generated cache
- `Temp/` - Temporary build files
- `UserSettings/` - Local user preferences
- `Logs/` - Diagnostic logs
- Any credential files

## Performance Notes

- Game targets 60 FPS (`Application.targetFrameRate = 60`)
- Auto-save every 5 seconds
- Stats sent to PlayFab every 10 minutes
- Passive income calculated per frame with realtime delta clamping
- Large number support via custom `Decimal512` system for game values
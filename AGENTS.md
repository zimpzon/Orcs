# Repository Guidelines

## Project Structure & Module Organization
- Unity project root at `C:\Repo\IdleKnight`; gameplay scripts live under `Assets/Script/` grouped by feature (e.g., `Managers`, `Weapons`, `Upgrades`).
- Art, audio, and prefabs reside in `Assets/Texture`, `Assets/Audio`, and `Assets/Prefab` respectively; keep build outputs in `Build/` and out of `Assets/`.
- Shared project configuration is stored in `ProjectSettings/`; avoid editing `Library/` or `Temp/` as they are generated.

## Build, Test, and Development Commands
- Launch the Unity Editor version recorded in `ProjectSettings/ProjectVersion.txt` to iterate on gameplay and UI.
- For scripted desktop builds, run `%UNITY_PATH% -projectPath "C:\Repo\IdleKnight" -quit -batchmode -executeMethod BuildPipeline.BuildPlayer` to produce binaries in `Build/`.
- Open `IdleKnight.sln` in Rider or Visual Studio to edit C# scripts and attach the debugger to the Unity Editor for play mode validation.

## Coding Style & Naming Conventions
- Use four-space indentation and follow Unity/C# defaults: PascalCase for classes, methods, and public fields; camelCase for locals and private members.
- Keep serialized fields declared at the top of each MonoBehaviour for inspector clarity and avoid redundant namespaces.
- Run the IDE formatter respecting `.editorconfig` before committing; prefer brief, purpose-driven comments only when logic is non-obvious.

## Testing Guidelines
- Automated tests are not maintained; rely on manual play mode validation and document reproduction steps when issues arise.
- When authoring future tests, place EditMode suites in `Assets/Tests/EditMode` and PlayMode suites in `Assets/Tests/PlayMode`; name test files after the feature under test (e.g., `UpgradePopupTests`).

## Commit & Pull Request Guidelines
- Write short, imperative commit messages (e.g., `Add upgrade popup flip logic`) and avoid committing generated `Library/` or `Temp/` assets.
- Pull requests should describe the change, list impacted scenes or prefabs, link task IDs if applicable, and include screenshots or GIFs for visual updates.

## Security & Configuration Tips
- Restore packages via the checked-in `Packages/manifest.json`; do not alter Unity cache directories.
- Configure PlayFab or other credentials through Unity’s editor extensions and keep secrets in `UserSettings/`, never in source control.

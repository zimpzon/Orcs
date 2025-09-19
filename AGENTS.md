# Repository Guidelines

## Project Structure & Module Organization
The Unity project root contains the source under Assets/. Gameplay scripts live in Assets/Script (grouped by feature directories like Managers, Weapons, Upgrades), art and audio assets in Assets/Texture, Assets/Audio, and prefabs in Assets/Prefab. Shared configuration sits in ProjectSettings/, while build artifacts should be produced in Build/ and kept out of Assets/.

## Build, Test, and Development Commands
Run the project in the Unity Editor matching the version recorded in ProjectSettings/ProjectVersion.txt. For scripted builds, use "%UNITY_PATH%" -projectPath "C:\Repo\IdleKnight" -quit -batchmode -executeMethod BuildPipeline.BuildPlayer to produce desktop players into Build/. Open IdleKnight.sln in Rider/Visual Studio for C# iteration; use the IDE's Attach to Unity integration for play mode debugging.

## Coding Style & Naming Conventions
Follow Unity/C# conventions: four-space indentation, PascalCase for classes and public members, camelCase for locals and private fields (no prefix unless Unity serialization requires public). Keep MonoBehaviour scripts lean—prefer feature folders over nested namespaces. Run the IDE formatter with .editorconfig defaults and ensure serialized fields remain at the top of the file for inspector clarity.

## Testing Guidelines
Automated tests are not maintained in this repo; ignore the Unity package-cache Tests/ directories that ship with dependencies. Focus on manual validation in play mode and document reproduction steps when filing bugs. When adding future tests, place EditMode suites under Assets/Tests/EditMode and PlayMode suites under Assets/Tests/PlayMode.

## Commit & Pull Request Guidelines
Commits are short, imperative summaries (e.g., dded three new enemies). Group related gameplay tweaks together and avoid committing generated Library/Temp files. Pull requests should include a concise change description, affected scenes or prefabs, any linked task IDs, and GIFs/screenshots demonstrating new UI or VFX behaviour.

## Environment & Configuration Tips
Use the checked-in Packages/manifest.json to restore dependencies and avoid modifying Library/. Configure PlayFab credentials via Unity's PlayFab editor extension and never commit keys; store secrets in your local UserSettings/ profile.
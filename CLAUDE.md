# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A Unity 3D stealth game ("Haunted House") built by following Unity's 3D stealth game tutorial. Requires **Unity 6000.7.0a3** (see `ProjectSettings/ProjectVersion.txt`). Uses the Universal Render Pipeline (URP) and the new Input System (`com.unity.inputsystem`).

There is no CLI build/test workflow — compilation, play mode, and builds all happen through the Unity Editor. After editing C# scripts, verify they compile by checking the Unity console (via the MCP tools below) rather than trying to build from the command line.

## Unity MCP Tools

A Unity MCP server is connected to the running Editor. Use it to:
- `Unity_GetConsoleLogs` — check for compile errors and runtime logs after making changes
- `Unity_RunCommand` — execute editor commands
- `Unity_SceneView_Capture2DScene` / `CaptureMultiAngleSceneView` / `Unity_Camera_Capture` — visually inspect the scene
- `Unity_AssetGeneration_*` — generate assets

## Project Layout

- `Assets/Scenes/Haunted House Scene.unity` — the main working scene where the game is being built.
- `Assets/_3DStealthGame/` — tutorial asset pack: art (models, materials, textures organized by room — Bathroom, Bedroom, Corridor, DiningRoom, Kitchen), audio, prefabs (`Prefabs/Environment/` for rooms/walls/decorations, `Prefabs/Levels/`), fonts, and a toon shader.
- `Assets/_3DStealthGame/Tutorial_Demo/` — the tutorial's **finished reference implementation**: completed scenes (`Demo_Scenes/DemoScene.unity`, `MainMenu.unity`), animators, prefabs, and scripts. Treat this as reference material, not the game itself.
- `Assets/InputSystem_Actions.inputactions` — input action definitions.
- `Assets/Settings/` — URP renderer/pipeline assets (separate PC and Mobile configurations) and volume profiles.

## Code Conventions

Game scripts live under the `StealthGame` namespace (see `Tutorial_Demo/Demo_Scripts/` for the reference style: `PlayerMovement`, `Observer`, `WaypointPatrol`, `GameEnding`, `Door`, `Key`, `MainMenu`). The demo scripts use `m_` prefixes for private fields, public fields for Inspector-exposed values, physics movement via `Rigidbody.MovePosition`/`MoveRotation` in `FixedUpdate`, and input via `InputAction` from the new Input System.

## Unity-Specific Rules

- Every asset and folder under `Assets/` has a paired `.meta` file containing its GUID. When creating, moving, renaming, or deleting assets, keep the `.meta` file in sync — never delete or regenerate one for an asset that is referenced elsewhere, or scene/prefab references will break.
- Scene (`.unity`), prefab (`.prefab`), and asset (`.asset`) files are Unity YAML. Prefer making scene/prefab changes through the Editor (via MCP) over hand-editing YAML.
- `Library/`, `Temp/`, `Logs/`, and `UserSettings/` are generated and git-ignored — never edit or search them for project code.

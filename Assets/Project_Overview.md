# Technical Overview: Haunted House Stealth Game

## 1. Project Description

This project is a 3D stealth-action prototype where players navigate a haunted environment while avoiding detection by supernatural entities. The core pillars of the experience are **spatial awareness**, **stealth movement**, and **reactive AI**. Players must reach a goal while staying out of the "line of sight" of patrolling enemies. The game features stylized 3D visuals using the Universal Render Pipeline (URP) and utilizes Unity's modern Behavior system for logic.

## 2. Gameplay Flow / User Loop

1.  **Boot/Main Menu**: The game starts in a Main Menu scene (UITK-based) where the player can begin the experience.
2.  **Exploration**: The player moves through the "Haunted House" scene using standard movement controls.
3.  **Stealth/Detection**: Enemies (Gargoyles, Ghosts) patrol on predefined paths. If the player enters a detection cone and has a clear line of sight, they are "Caught."
4.  **Reset Loop**: Upon being caught, the UI fades to a "Caught Screen," the player is teleported back to the start, and control is restored once the screen fades back out.
5.  **Win Condition**: Reaching the end of the level triggers an "End Screen."

## 3. Architecture

The project follows a **Decoupled Event-Driven Architecture** combined with **Behavior Graphs**.

- **Global Event Bus**: The `GameEvents` static class serves as the central communication hub. It allows disparate systems (like UI and Player) to communicate without direct references.
- **Behavior Graphs**: Player and potentially AI logic are driven by the `Unity.Behavior` package. This moves high-level state logic (Waiting for Input -> Walking) into visual graphs, while actions are implemented in C#.
- **Input Handling**: A dedicated `BehaviorInputReader` bridges the New Input System and the Behavior Graph.
- **Separation of Concerns**: Visuals (Animations), Logic (Behavior), and Physical detection (Colliders) are kept in separate components.

`Location: Assets/Source/`

## 4. Game Systems & Domain Concepts

### Movement System

Driven by a `CharacterController` and controlled via the Behavior Graph.

- `WalkAction`: A custom Behavior Action that translates input into physical movement and updates Animator parameters.
- `WaitForMoveInputAction`: A Behavior node that halts execution until movement input is detected.
- `PlayerController`: Handles high-level lifecycle events like respawning and disabling the `CharacterController` when caught.

`Location: Assets/Source/Player/`

### Enemy & Detection System

Enemies move along Splines and use trigger-based raycasting for sight.

- `PlayerDetector`: Uses a Trigger to define a field of view and `Physics.Linecast` to check for solid obstructions between the enemy and the player.
- `LookTowardSpline`: Uses the `Unity.Splines` package to sample future positions on a path, allowing enemies to face their direction of travel smoothly.
- `SplineAnimate`: (Unity Component) Handles the physical movement along the spline.

`Location: Assets/Source/Enemy/`

### Event System

The glue of the project, facilitating state transitions across systems.

- `GameEvents`: Contains events like `Caught`, `FadeInComplete`, and `FadeScreenReset`.

`Location: Assets/Source/GameEvents.cs`

## 5. Scene Overview

- **Haunted House Scene**: The primary gameplay level containing the environment, splines for enemy patrols, and the player prefab.
- **MainMenu**: Initial entry point using UI Toolkit for navigation.
- **Tutorial/Demo Scenes**: Contained within `Assets/_3DStealthGame/Tutorial_Demo`, these serve as reference implementations.

`Location: Assets/Scenes/` and `Assets/_3DStealthGame/Tutorial_Demo/Demo_Scenes/`

## 6. UI System

The project uses **UI Toolkit (UITK)** for its interface.

- **Main UI**: Managed by the `Main` class, which queries the `VisualTreeAsset` (UXML) for elements like "EndScreen" and "CaughtScreen."
- **Screen Transitions**: UI fading is handled via coroutines in the `Main` script that manipulate the `style.opacity` of VisualElements.
- **Binding**: The UI listens to `GameEvents.Caught` to trigger the failure overlay.

`Location: Assets/Source/UI/` and `Assets/_3DStealthGame/UI/`

## 7. Asset & Data Model

- **Behavior Graphs**: `Player.asset` (BehaviorAuthoringGraph) defines the logic flow for the player character.
- **Splines**: Patrol paths are stored as Spline components within the scene, allowing for easy path editing in the Editor.
- **Prefabs**: Character setups (Player, Ghost, Gargoyle) are modularized in the `Assets/Prefabs/` folder.
- **URP Settings**: Graphics configurations are split into PC and Mobile variants in the `Settings/` folder.

## 8. Notes, Caveats & Gotchas

- **CharacterController Conflict**: When teleporting the player (e.g., respawning), the `CharacterController` must be disabled. If not, its cached position may override the manual transform change.
- **Line of Sight Layers**: The `PlayerDetector` uses `QueryTriggerInteraction.Ignore`. Ensure that detection volumes (Triggers) are on the correct layer or that the detector is configured to ignore its own collider to prevent self-blocking.
- **Static Events**: Since `GameEvents` is static, UI and Player components must strictly unsubscribe in `OnDisable` or `OnDestroy` to prevent memory leaks and null reference exceptions when switching scenes.
- **Behavior Input**: The Behavior Graph doesn't read input directly; it relies on `BehaviorInputReader` to cache the `MoveDirection` from the New Input System.

## 9. Style, Quality & Testing

- Every C# file under `Assets/Source/` gets a namespace matching its directory relative to `Source` (e.g. `Source/Player/Combat` → `namespace Source.Player.Combat`).
- Unless specified otherwise, assume every component is an empty class. Do not add methods, properties or fields.
- Ignore any code inside the `_3DStealthGame` folder for any guidance on code formatting, quality and practices.
- Prefer to define properties as public with private setters.
- Prefer using events over polling when working with Unity’s input system.
- Private fields should be prefixed with an `_` character.
- Fields or properties storing a component should be suffixed with `Component`. For example, if a field is storing an `Animator` component, the field should be called `_animatorComponent`.
- Prefer to use hash IDs when working with animator parameters. Store the IDs in readonly fields.
- Prefer to use `OnSetup()` for setting up components in the `Unity.Behavior.Action` class.
- Prefer guard clauses over nested conditionals.
- Prefer `UnityAction` over `Action`, including for event types; always use the `event` keyword.
- Every event gets a static `Raise<EventName>()` invoker method; handlers are named `On<EventName>`.
- Do not update the scene in the game unless explicitily told to do so.

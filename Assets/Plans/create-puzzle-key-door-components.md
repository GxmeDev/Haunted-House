# Project Overview
- Game Title: Haunted House
- High-Level Concept: A 3D stealth-action prototype where players navigate a haunted environment while avoiding detection by supernatural entities.
- Players: Single player
- Inspiration / Reference Games: Stealth horror / puzzle exploration games
- Tone / Art Direction: Stylized 3D visuals using URP
- Target Platform: PC (StandaloneWindows64)
- Screen Orientation / Resolution: Landscape
- Render Pipeline: Universal Render Pipeline (URP)

# Game Mechanics
## Core Gameplay Loop
Players explore a haunted environment, collecting key items and opening matching doors while staying out of enemy lines of sight.
## Controls and Input Methods
Standard 3D movement using New Input System driven by Unity Behavior Graph.

# UI
Managed by UI Toolkit in `Assets/Source/UI/` for screen fades and game state overlays.

# Key Asset & Context
- `Assets/Source/Puzzle/KeySO.cs`: ScriptableObject script defining the key data asset.
- `Assets/Source/Puzzle/Key.cs`: MonoBehaviour component representing a key item in the world holding a reference to `KeySO`.
- `Assets/Source/Puzzle/Door.cs`: MonoBehaviour component representing a door in the world holding a reference to `KeySO`.

Coding conventions observed:
- Namespace: `Source.Puzzle`
- Private field prefix: `_` (`[SerializeField] private KeySO _keyData;`)
- Empty classes unless specified otherwise (no extra unused methods/fields).

# Implementation Steps

### Step 1: Create `KeySO.cs`
- **Description**: Create `Assets/Source/Puzzle/KeySO.cs` defining `KeySO` ScriptableObject in namespace `Source.Puzzle` with `[CreateAssetMenu]`.
- **Assigned role**: developer
- **Dependencies**: None
- **Parallelizable**: Yes

### Step 2: Create `Key.cs`
- **Description**: Create `Assets/Source/Puzzle/Key.cs` defining `Key` MonoBehaviour component in namespace `Source.Puzzle` with `[SerializeField] private KeySO _keyData;`.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

### Step 3: Create `Door.cs`
- **Description**: Create `Assets/Source/Puzzle/Door.cs` defining `Door` MonoBehaviour component in namespace `Source.Puzzle` with `[SerializeField] private KeySO _keyData;`.
- **Assigned role**: developer
- **Dependencies**: Step 1
- **Parallelizable**: No

### Step 4: Compilation Verification
- **Description**: Verify the Unity Editor compiles all new scripts without errors or warnings.
- **Assigned role**: developer
- **Dependencies**: Steps 1, 2, 3
- **Parallelizable**: No

# Verification & Testing
- Check console logs via `Unity.GetConsoleLogs` to confirm 0 compilation errors.
- Confirm file paths exist at `Assets/Source/Puzzle/KeySO.cs`, `Assets/Source/Puzzle/Key.cs`, and `Assets/Source/Puzzle/Door.cs`.
- Ensure namespace `Source.Puzzle` and field `_keyData` match project standards.

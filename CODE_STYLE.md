# Code Styles and Quality

- Every C# file under `Assets/Source/` gets a namespace matching its directory relative to `Source` (e.g. `Source/Player/Combat` → `namespace Source.Player.Combat`).
- Unless specified otherwise, assume every component is an empty class. Do not add methods, properties or fields.
- Ignore any code inside the `_3DStealthGame` folder for any guidance on code formatting, quality and practices.
- Prefer to define properties as public with private setters.
- Prefer using events over polling when working with Unity’s input system.

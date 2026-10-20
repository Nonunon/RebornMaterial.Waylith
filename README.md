# RebornMaterial.Waylith

A port of [RebornMaterial](https://github.com/LTS-FFXIV/RebornMaterial) by LTS (the CombatReborn team) from Dalamud ImGui to the Dear ImGui that RimWorld 1.7 ships (`Brutal.ImGui` on the Waylith engine). Material Design 3 colours and shapes for Dear ImGui windows drawn from a RimWorld mod.

Licensed under the GNU LGPL v3, like the original (`LICENSE`; the GPL v3 it builds on is in `LICENSE.GPL-3.0`). Ported from upstream commit `8bb6a93` (2026-10-20). The first commit in this repo holds upstream's files unchanged, so `git diff` against it shows every change made for the port.

## What is ported

| File | State |
| --- | --- |
| `M3Palette.cs` | Unchanged apart from vector types |
| `M3.cs` | Dalamud plugin setup removed; scale follows RimWorld's UI scale; font roles are `M3Font` |
| `M3Style.cs` | ImGui 1.92 colour names (`TabSelected`, `TabDimmed`, `NavCursor`, ...), plus the 1.92 slots for tab overlines, links, the text cursor and tree lines |
| `M3Draw.cs` | Icon and texture helpers left out for now |
| `M3Settings.cs` | Unchanged apart from vector types |
| `M3Fonts.cs` | Rewritten: the game's font at any size through `PushFont(font, size)` |
| `ImRaii.cs` | New: push and pop helpers in the style of Dalamud's `ImRaii` |

Other changes across files: `Vector2`/`Vector4` are `float2`/`float4`, colours are `ImColor8`, and `ImGui.TextUnformatted` is `ImGui.Text`.

Not ported yet: widgets, cards, navigation, menus, dialogs, snackbars, charts, the tree, the window fold, the gallery, icons and game textures.

## Using it

Reference the project from a mod's csproj; the DLL is copied into the mod's `Assemblies` folder. Call `M3.Initialize(settings)` once, `M3.BeginFrame()` each frame, and wrap the window in `using (M3Style.Push()) { ... }`. A mod can ship this DLL next to its own under any licence, as long as the DLL stays a separate file and its source (with any changes) stays available under the LGPL v3.

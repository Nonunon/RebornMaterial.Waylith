# RebornMaterial.Waylith

A port of [RebornMaterial](https://github.com/LTS-FFXIV/RebornMaterial) by LTS (the CombatReborn team) from Dalamud ImGui to the Dear ImGui that RimWorld 1.7 ships (`Brutal.ImGui` on the Waylith engine). Material Design 3 colours and shapes for Dear ImGui windows drawn from a RimWorld mod.

Licensed under the GNU LGPL v3, like the original (`LICENSE`; the GPL v3 it builds on is in `LICENSE.GPL-3.0`). Ported from upstream commit `8bb6a93` (2026-10-20). The first commit in this repo holds upstream's files unchanged, so `git diff` against it shows every change made for the port.

## What is ported

Every upstream file. Most only needed the type changes listed below the table.

| File | State |
| --- | --- |
| `M3Palette.cs`, `M3Settings.cs` | Unchanged apart from vector types |
| `M3.cs` | Dalamud plugin setup removed; scale follows RimWorld's UI scale; font roles are `M3Font` |
| `M3Style.cs` | ImGui 1.92 colour names (`TabSelected`, `TabDimmed`, `NavCursor`, ...), plus the 1.92 slots for tab overlines, links, the text cursor and tree lines |
| `M3Draw.cs` | Icons draw with the bundled icon font |
| `M3Widgets.cs`, `M3Widgets.NumberField.cs` | Text inputs go through `M3Input`; button repeat uses `PushItemFlag` |
| `M3Widgets.Hotkey.cs` | `M3Hotkey` holds a Waylith `Key` instead of Dalamud's `VirtualKey` |
| `M3Widgets.ReorderList.cs` | Private button flags and the resize cursor use their Brutal names |
| `M3WindowFold.cs` | Drives an `M3WindowState` instead of a Dalamud `Window`; sizes are in pixels |
| `M3ActionIcon.cs` | FFXIV game-icon lookup removed; images take an `IM3Texture` |
| `M3Gallery.cs` | Demo icons are RimWorld item icons; `Page`, `PageCount` and `PageName` added so a host can pick a page |
| `M3Dialog.cs` | Wrapped text escapes `%` |
| `M3Card.cs`, `M3Navigation.cs`, `M3Menu.cs`, `M3SettingRow.cs`, `M3Text.cs`, `M3TextField.cs`, `M3Tooltip.cs`, `M3Badge.cs`, `M3Flow.cs`, `M3Tree.cs`, `M3Snackbar.cs`, `M3Chart.cs`, `M3HeroHeader.cs` | Unchanged apart from types and fonts |
| `M3Fonts.cs` | Rewritten: the game's font at any size through `PushFont(font, size)` |
| `M3Icons.cs` | New: the FontAwesome icons the library uses (codepoints match Dalamud's `FontAwesomeIcon`), loaded from an embedded Font Awesome Free Solid 7.1 font |
| `M3Input.cs` | New: `ref string` text input on top of Brutal's `ImInputString` |
| `M3Host.cs` | New: passes the mouse, keys, modifiers, typed characters and window focus from Waylith to ImGui, once per frame |
| `M3Clipboard.cs` | New: clipboard through Waylith, installed only when ImGui has none of its own (the Windows `imgui.dll` does) |
| `M3TypingGuard.cs` | New: stops RimWorld's hotkeys and camera keys while an ImGui text field has focus |
| `M3Texture.cs`, `M3GameTexture.cs` | New: `IM3Texture` stands in for Dalamud's texture wrap; `M3GameTexture` wraps any RimWorld `Texture2D` or `RenderTexture` |
| `M3WindowState.cs` | New: position, size and flags for the window fold |
| `ImRaii.cs` | New: push and pop helpers in the style of Dalamud's `ImRaii` |

Other changes across files: `Vector2`/`Vector4` are `float2`/`float4`, colours are `ImColor8`, `ImGuiP` is `ImGui.Internal`, `ImGui.TextUnformatted` is `ImGui.Text`, and `TextWrapped` escapes `%` (Brutal passes it to printf).

The icon font is Font Awesome Free by Fonticons, Inc., under the SIL OFL 1.1 (`LICENSE.FontAwesome.txt`).

## Building

Needs the .NET 10 SDK and RimWorld 1.7. The project references the game's own DLLs (`Assembly-CSharp`, `UnityStubs`, `Waylith`, `Brutal.*`) from the game folder; if RimWorld is not at `F:\Steam\steamapps\common\RimWorld`, build with `dotnet build -c Release -p:RimWorld17Dir=<your RimWorld folder>`. None of the game's files are included here.

## Using it

Reference the project from a mod's csproj; the DLL is copied into the mod's `Assemblies` folder. A mod can ship this DLL next to its own under any licence, as long as the DLL stays a separate file and its source (with any changes) stays available under the LGPL v3.

A Dear ImGui window drawn from a RimWorld mod:

1. Call `M3.Initialize(settings)` once.
2. In a `Verse.Window`'s `ExtraOnGUI`, during the Repaint event only: call `M3Host.FeedInput()`, then `M3.BeginFrame()`, then draw inside `using (M3Style.Push()) { ... }` between `ImGui.Begin` and `ImGui.End`.
3. For text fields, give the window an `M3TypingGuard`: return its `Widget` from `CommonSearchWidget`, return `M3TypingGuard.HiddenRect` from `QuickSearchWidgetRect`, and call `Update()` each frame. RimWorld skips its hotkeys while a search widget has focus, and the guard focuses a hidden one while ImGui wants typing.
4. Dear ImGui always draws on top of RimWorld's own GUI, so skip the ImGui window while `M3Host.IsCovered(window)` is true (a RimWorld window above yours in the stack overlaps it or blocks input, such as a confirmation dialog), and call `M3Host.ReleaseMouse()` instead.
5. Wrap game textures in `M3GameTexture` to pass them to any component that takes an `IM3Texture`. It uses a private method of the game's GUI backend, so a future build could break it; it then logs one warning and draws nothing.

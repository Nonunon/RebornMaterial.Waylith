# RebornMaterial.Waylith

Material Design 3 for the Dear ImGui that ships with RimWorld 1.7 (`Brutal.ImGui` on the Waylith engine). A port of [RebornMaterial](https://github.com/LTS-FFXIV/RebornMaterial) by LTS and the CombatReborn team, which was written for Dalamud (FFXIV plugins).

One accent colour generates the whole scheme. Included: buttons, switches, checkboxes, radios, chips, sliders, text, number, search and hotkey fields, cards, tabs, a navigation drawer and rail, menus, trees, dialogs, snackbars, banners, charts, and a gallery page that shows all of them.

## Licence

LGPL v3, like the original (`LICENSE`, with the GPL v3 it builds on in `LICENSE.GPL-3.0`). A mod can ship the DLL beside its own code under any licence, as long as the DLL stays a separate file and its source, with any changes, stays available under the LGPL.

The bundled icon font is Font Awesome Free by Fonticons, Inc., under the SIL OFL 1.1 (`LICENSE.FontAwesome.txt`).

## Building

Needs the .NET 10 SDK and RimWorld 1.7. The project references the game's own DLLs from the RimWorld folder; none of them are included here. If RimWorld is not at the default Steam location, create `Local.props` next to this README:

```xml
<Project>
  <PropertyGroup>
    <RimWorld17Dir>D:\Games\Steam\steamapps\common\RimWorld</RimWorld17Dir>
  </PropertyGroup>
</Project>
```

or pass `-p:RimWorld17Dir=<folder>` to `dotnet build`.

## Using it

Reference the project from your mod's csproj, so the DLL is copied into the mod's `Assemblies` folder. RimWorld has no ImGui windows of its own, so a mod hosts one inside a transparent `Verse.Window`:

```csharp
public class MyWindow : Window
{
    private readonly M3TypingGuard typing = new M3TypingGuard();

    public override QuickSearchWidget CommonSearchWidget => typing.Widget;

    protected override Rect QuickSearchWidgetRect(Rect winRect, Rect inRect) => M3TypingGuard.HiddenRect;

    public override void ExtraOnGUI()
    {
        if (Event.current.type != EventType.Repaint)
            return;
        M3Host.FeedInput();
        typing.Update();
        if (M3Host.ShouldHide(this))
        {
            M3Host.ReleaseMouse();
            return;
        }
        M3.BeginFrame();
        using (M3Style.Push())
        {
            bool visible = ImGui.Begin("My window");
            M3Host.Claim(this);
            if (visible)
                M3Widgets.Button("##hello", "Hello", M3ButtonStyle.Filled);
            ImGui.End();
        }
    }
}
```

Call `M3.Initialize(settings)` once at startup. Set the window's `windowRect` to the ImGui window's position and size (divided by `Prefs.UIScale`) so clicks on it don't fall through to the map.

- `M3Host.FeedInput` passes the mouse, keys, typed characters and window focus to ImGui; the game itself feeds it nothing.
- `M3TypingGuard` stops RimWorld's hotkeys and camera keys while an ImGui text field has focus. RimWorld skips hotkeys while a window's search box has focus, so the guard focuses a hidden one.
- ImGui always draws on top of RimWorld's own windows. If the [Underlayer](https://github.com/Nonunon/Underlayer) mod is installed, `M3Host.Claim` lets it stack your ImGui window with RimWorld's windows, so a dialog opened later covers it. Without Underlayer, `M3Host.ShouldHide` (the same as `M3Host.IsCovered`) reports when a RimWorld window above yours overlaps it or blocks input, so you can hide it instead. `M3Host.Layered` says which one applies. `M3Host.Claim(this, true)` passes Underlayer's `riseAboveDialogs`, so a click lifts your window above letters and confirmation boxes until another window takes over. Underlayer is found at runtime, so your mod doesn't need to reference it.
- `M3GameTexture` wraps any RimWorld `Texture2D` or `RenderTexture` (item icons, pawn portraits) for components that take an image. It relies on a private method of the game's GUI backend, so a later game build could break it; it then logs one warning and draws nothing.
- Brutal passes some text straight to printf, so a `%` in `ImGui.TextWrapped`, `TextColored` or `TextDisabled` crashes the game. Escape it as `%%`, or use `ImGui.Text`.

## Changes from upstream

Ported from upstream commit `8bb6a93`. The first commit here holds upstream's files unchanged, so `git diff` against it shows every change.

- Everywhere: `Vector2`/`Vector4` are `float2`/`float4`, colours are `ImColor8`, `ImGuiP` is `ImGui.Internal`, `ImGui.TextUnformatted` is `ImGui.Text`, wrapped text escapes `%`, and comments were shortened.
- `M3.cs`: no Dalamud plugin setup; sizes follow RimWorld's UI scale; font roles are `M3Font`.
- `M3Fonts.cs`: rewritten to use the game's font at any size.
- `M3Style.cs`: ImGui 1.92 colour names, plus the newer tab overline, link, text cursor and tree line colours.
- `M3Widgets.Hotkey.cs`: hotkeys hold a Waylith `Key`.
- `M3WindowFold.cs`: drives an `M3WindowState` instead of a Dalamud `Window`.
- `M3ActionIcon.cs`: no FFXIV icon lookup; images take an `IM3Texture`.
- `M3Gallery.cs`: RimWorld item icons as demo images; `Page`, `PageCount` and `PageName` let a host pick a page.
- New files: `M3Host`, `M3TypingGuard`, `M3Clipboard` (only used when ImGui has no clipboard of its own), `M3GameTexture`, `M3Texture`, `M3Icons` (the embedded icon font), `M3Input`, `M3WindowState` and `ImRaii`.

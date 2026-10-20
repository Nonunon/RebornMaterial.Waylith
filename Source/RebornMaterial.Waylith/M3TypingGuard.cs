using RimWorld;

namespace RebornMaterial;

public sealed class M3TypingGuard
{
	public QuickSearchWidget Widget { get; } = new();

	public static UnityEngine.Rect HiddenRect => new(-10000f, -10000f, 200f, 24f);

	public bool Active => Widget.CurrentlyFocused();

	public void Update()
	{
		Widget.Reset();
		var wanted = ImGui.GetIO().WantTextInput;
		if (wanted && !Widget.CurrentlyFocused())
		{
			Widget.Focus();
		}
		else if (!wanted && Widget.CurrentlyFocused())
		{
			Widget.Unfocus();
		}
	}
}

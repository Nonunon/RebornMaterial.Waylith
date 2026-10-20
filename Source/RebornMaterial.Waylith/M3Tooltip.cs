using Dalamud.Interface.Utility.Raii;

namespace RebornMaterial;

public static class M3Tooltip
{
	// In multiples of the font size.
	private const float WrapEms = 28f;

	/// <summary>Replaces the built-in tooltip for every component, e.g. to honour a plugin's own "show tooltips" setting.</summary>
	public static Action<string>? Handler { get; set; }

	public static void Hovered(string? text)
	{
		if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
		{
			Show(text);
		}
	}

	public static void Show(string? text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return;
		}

		if (Handler != null)
		{
			Handler(text);
			return;
		}

		using var tooltip = ImRaii.Tooltip();
		using var wrap = ImRaii.TextWrapPos(ImGui.GetFontSize() * WrapEms);
		ImGui.TextUnformatted(text);
	}
}

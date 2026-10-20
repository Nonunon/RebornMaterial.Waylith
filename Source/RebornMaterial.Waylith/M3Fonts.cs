namespace RebornMaterial;

public readonly record struct M3Font(ImFontPtr Font, float Size);

public static class M3Fonts
{
	public static float DefaultSizePx { get; set; } = 15f;

	public static M3Font GetFont(float size)
	{
		return new M3Font(ImGui.GetFont(), MathF.Max(1f, MathF.Round(size * Verse.Prefs.UIScale)));
	}

	public static M3Font GetDefaultFont(float scale)
	{
		return GetFont(DefaultSizePx * scale);
	}

	public static void Push(M3Font font)
	{
		var style = ImGui.GetStyle();
		ImGui.PushFont(font.Font, font.Size / MathF.Max(0.01f, style.FontScaleMain * style.FontScaleDpi));
	}
}

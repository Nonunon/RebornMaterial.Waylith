namespace RebornMaterial;

/// <summary>A plugin's own configuration can implement this directly, so the theme follows its saved settings.</summary>
public interface IM3Settings
{
	/// <summary>The seed the whole color scheme is derived from. Grey or black falls back to <see cref="M3.DefaultSeed"/>.</summary>
	Vector4 UiAccentColor { get; set; }

	/// <summary>Text size multiplier, clamped to 0.5 - 3.</summary>
	float UiTextScale { get; set; }

	/// <summary>Element size multiplier, clamped to 0.5 - 2.5.</summary>
	float UiElementScale { get; set; }

	/// <summary>Padding and spacing multiplier, on top of the element size, clamped to 0 - 3. Settings that don't store it stay at 1.</summary>
	float UiPaddingScale
	{
		get => 1f;
		set { }
	}
}

public sealed class M3Settings : IM3Settings
{
	public Vector4 UiAccentColor { get; set; } = M3.DefaultSeed;

	public float UiTextScale { get; set; } = 1f;

	public float UiElementScale { get; set; } = 1f;

	public float UiPaddingScale { get; set; } = 1f;
}

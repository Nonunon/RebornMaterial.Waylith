namespace RebornMaterial;

/// <summary>Implement on a mod's settings so the theme follows them.</summary>
public interface IM3Settings
{
	/// <summary>Seed colour for the whole scheme. Grey or black falls back to <see cref="M3.DefaultSeed"/>.</summary>
	float4 UiAccentColor { get; set; }

	/// <summary>Text size multiplier, clamped to 0.5 - 3.</summary>
	float UiTextScale { get; set; }

	/// <summary>Element size multiplier, clamped to 0.5 - 2.5.</summary>
	float UiElementScale { get; set; }

	/// <summary>Padding and spacing multiplier on top of the element size, clamped to 0 - 3. Defaults to 1.</summary>
	float UiPaddingScale
	{
		get => 1f;
		set { }
	}
}

public sealed class M3Settings : IM3Settings
{
	public float4 UiAccentColor { get; set; } = M3.DefaultSeed;

	public float UiTextScale { get; set; } = 1f;

	public float UiElementScale { get; set; } = 1f;

	public float UiPaddingScale { get; set; } = 1f;
}

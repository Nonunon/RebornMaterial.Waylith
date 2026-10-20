namespace RebornMaterial;

public static class M3
{
	public static readonly float4 DefaultSeed = M3ColorMath.FromRgb(0xB0201F);

	private static bool _initialized;

	// Tracks the configured seed, not the fallback, so a rejected colour isn't rebuilt on every access.
	private static float4 _configuredSeed = DefaultSeed;
	private static M3Scheme _scheme = M3Scheme.FromSeed(DefaultSeed);

	private static float _windowScale = 1f;

	private static float _elementScale = 1f;

	private static float _paddingScale = 1f;

	/// <summary>Where the accent colour and the size settings are read from.</summary>
	public static IM3Settings Settings { get; set; } = new M3Settings();

	/// <summary>Element size at 100%. Below 1 is more compact than the Material spec.</summary>
	public static float ElementBaseline { get; set; } = 1f;

	public static bool IsInitialized => _initialized;

	/// <summary>Call once from the mod's constructor, before anything is drawn.</summary>
	public static void Initialize(IM3Settings? settings = null)
	{
		_initialized = true;
		if (settings != null)
		{
			Settings = settings;
		}

		ReadSizes();
	}

	/// <summary>Clears every cached animation and layout.</summary>
	public static void Dispose()
	{
		M3Snackbar.Clear();
		M3Motion.Reset();
		M3CardHost.Reset();
		M3Widgets.Reset();
		M3Menu.Reset();
		M3Tree.Reset();
		M3Navigation.Reset();
		M3Tooltip.Handler = null;
		_initialized = false;
	}

	public static float Scale => Verse.Prefs.UIScale * _elementScale * _windowScale;

	/// <summary><see cref="Scale"/> times the padding setting, so spacing can change without resizing controls.</summary>
	public static float PaddingScale => Scale * _paddingScale;

	/// <summary>Call once per frame, before any window draws.</summary>
	// Sizes hold still while a control is held, so a size slider doesn't move under the mouse.
	public static void BeginFrame()
	{
		if (!ImGui.IsAnyItemActive())
		{
			ReadSizes();
		}
	}

	private static void ReadSizes()
	{
		_elementScale = ElementBaseline * Math.Clamp(Settings.UiElementScale, 0.5f, 2.5f);
		_paddingScale = Math.Clamp(Settings.UiPaddingScale, 0f, 3f);
	}

	public static float TextScale => Math.Clamp(Settings.UiTextScale, 0.5f, 3f) * _windowScale;

	// Push before M3Style.Push so the window's padding and corners scale too.
	public static WindowScaleScope PushWindowScale(float scale)
	{
		var previous = _windowScale;
		_windowScale = previous * Math.Clamp(scale, 0.1f, 10f);
		return new WindowScaleScope(previous);
	}

	public static M3Scheme Scheme
	{
		get
		{
			var seed = Settings.UiAccentColor;
			if (seed.X != _configuredSeed.X || seed.Y != _configuredSeed.Y || seed.Z != _configuredSeed.Z)
			{
				_configuredSeed = seed;

				// Grey or black seeds make an unusable theme.
				M3ColorMath.ToLch(seed, out var lightness, out var chroma, out _);
				_scheme = M3Scheme.FromSeed(lightness < 5f || chroma < 2f ? DefaultSeed : seed);
			}

			return _scheme;
		}
	}

	public static float ShapeExtraSmall => 4f * Scale;
	public static float ShapeSmall => 8f * Scale;
	public static float ShapeMedium => 12f * Scale;
	public static float ShapeLarge => 16f * Scale;
	public static float ShapeExtraLarge => 28f * Scale;
	public static float ShapeFull => 999f;

	public static float Space1 => 4f * PaddingScale;
	public static float Space2 => 8f * PaddingScale;
	public static float Space3 => 12f * PaddingScale;

	public const float StateHover = 0.08f;
	public const float StatePressed = 0.10f;
	public const float DisabledContent = 0.38f;
	public const float DisabledContainer = 0.12f;

	public static M3Font Body => M3Fonts.GetDefaultFont(TextScale);
	public static M3Font HeadlineSmall => M3Fonts.GetFont(22f * TextScale);
	public static M3Font TitleLarge => M3Fonts.GetFont(19f * TextScale);
	public static M3Font TitleMedium => M3Fonts.GetFont(16f * TextScale);
	public static M3Font LabelSmall => M3Fonts.GetFont(11f * TextScale);

	public static FontScope PushBody()
	{
		M3Fonts.Push(Body);
		return new FontScope(true);
	}

	public static float FitText(float height, float padding)
	{
		return MathF.Max(height * Scale, ImGui.GetTextLineHeight() + (padding * 2f * Scale));
	}

	public static float4 Alpha(float4 color, float alpha)
	{
		return color with { W = alpha };
	}

	public static float4 StateLayer(float4 container, float4 content, bool hovered, bool active)
	{
		if (!hovered && !active)
		{
			return container;
		}

		var opacity = active ? StatePressed + StateHover : StateHover;
		var mixed = M3ColorMath.Mix(container, content, opacity);
		return mixed with { W = container.W };
	}

	public static float4 ContentOn(float4 fill)
	{
		M3ColorMath.ToLch(fill, out var lightness, out _, out _);
		return lightness > 60f ? Scheme.Surface : Scheme.OnSurface;
	}

	public static ImColor8 U32(float4 color)
	{
		return ImGui.GetColorU32(color);
	}

	public static ImColor8 U32(float4 color, float alpha)
	{
		return ImGui.GetColorU32(color with { W = alpha });
	}

	// Turns a colour's hue up to 15 degrees toward the accent, so it sits better in the scheme.
	public static float4 Harmonize(float4 color)
	{
		M3ColorMath.ToLch(color, out var lightness, out var chroma, out var hue);
		M3ColorMath.ToLch(Scheme.Primary, out _, out _, out var target);

		var difference = M3ColorMath.WrapHue(target - hue + 180f) - 180f;
		var turn = MathF.Min(MathF.Abs(difference) * 0.5f, 15f) * MathF.Sign(difference);
		return M3ColorMath.FromLch(lightness, chroma, M3ColorMath.WrapHue(hue + turn)) with { W = color.W };
	}

	public static M3ColorRoles CustomColor(float4 color, bool harmonize = true)
	{
		if (harmonize)
		{
			color = Harmonize(color);
		}

		M3ColorMath.ToLch(color, out _, out var chroma, out var hue);
		var palette = new TonalPalette(hue, MathF.Max(chroma, 16f));
		return new M3ColorRoles(palette[80], palette[20], palette[30], palette[90]);
	}

	public static float4 Severity(M3Severity severity)
	{
		var scheme = Scheme;
		return severity switch
		{
			M3Severity.Error => scheme.Error,
			M3Severity.Warning => scheme.Warning,
			M3Severity.Success => scheme.Success,
			M3Severity.Info => scheme.Info,
			_ => scheme.Primary,
		};
	}

	public static float4 SeverityContainer(M3Severity severity)
	{
		var scheme = Scheme;
		return severity switch
		{
			M3Severity.Error => scheme.ErrorContainer,
			M3Severity.Warning => scheme.WarningContainer,
			M3Severity.Success => scheme.SuccessContainer,
			M3Severity.Info => scheme.SecondaryContainer,
			_ => scheme.PrimaryContainer,
		};
	}

	public readonly struct WindowScaleScope(float previous) : IDisposable
	{
		public void Dispose()
		{
			// A default scope pushed nothing, so leave the scale alone.
			if (previous > 0f)
			{
				_windowScale = previous;
			}
		}
	}

	public readonly struct FontScope(bool pushed) : IDisposable
	{
		public void Dispose()
		{
			if (pushed)
			{
				ImGui.PopFont();
			}
		}
	}
}

public readonly record struct M3ColorRoles(float4 Color, float4 OnColor, float4 Container, float4 OnContainer);

public enum M3Severity
{
	Neutral,
	Info,
	Success,
	Warning,
	Error,
}

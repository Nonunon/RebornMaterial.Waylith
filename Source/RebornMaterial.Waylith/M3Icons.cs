namespace RebornMaterial;

public enum FontAwesomeIcon
{
	None = 0x0,
	ArrowsAltH = 0xF337,
	Ban = 0xF05E,
	Bell = 0xF0F3,
	BellSlash = 0xF1F6,
	Bolt = 0xF0E7,
	Bug = 0xF188,
	CaretDown = 0xF0D7,
	ChartBar = 0xF080,
	Check = 0xF00C,
	CheckCircle = 0xF058,
	CheckSquare = 0xF14A,
	ChevronLeft = 0xF053,
	ChevronRight = 0xF054,
	Cog = 0xF013,
	Coins = 0xF51E,
	Compass = 0xF14E,
	Copy = 0xF0C5,
	Crosshairs = 0xF05B,
	Dragon = 0xF6D5,
	EllipsisH = 0xF141,
	Envelope = 0xF0E0,
	ExclamationCircle = 0xF06A,
	ExclamationTriangle = 0xF071,
	ExternalLinkAlt = 0xF35D,
	Eye = 0xF06E,
	EyeSlash = 0xF070,
	Filter = 0xF0B0,
	Fire = 0xF06D,
	Fish = 0xF578,
	FistRaised = 0xF6DE,
	Gem = 0xF3A5,
	Globe = 0xF0AC,
	GripVertical = 0xF58E,
	Hammer = 0xF6E3,
	HandPointer = 0xF25A,
	Hashtag = 0xF292,
	Heart = 0xF004,
	Home = 0xF015,
	Inbox = 0xF01C,
	InfoCircle = 0xF05A,
	Keyboard = 0xF11C,
	LayerGroup = 0xF5FD,
	Leaf = 0xF06C,
	Lightbulb = 0xF0EB,
	ListUl = 0xF0CA,
	Lock = 0xF023,
	LockOpen = 0xF3C1,
	Magic = 0xF0D0,
	MapMarkerAlt = 0xF3C5,
	Minus = 0xF068,
	Moon = 0xF186,
	MousePointer = 0xF245,
	Palette = 0xF53F,
	Pause = 0xF04C,
	Pen = 0xF304,
	Play = 0xF04B,
	Plus = 0xF067,
	QuestionCircle = 0xF059,
	Search = 0xF002,
	Seedling = 0xF4D8,
	Shapes = 0xF61F,
	ShareAlt = 0xF1E0,
	SlidersH = 0xF1DE,
	Smile = 0xF118,
	Snowflake = 0xF2DC,
	Square = 0xF0C8,
	Star = 0xF005,
	Sun = 0xF185,
	Sync = 0xF021,
	TachometerAlt = 0xF3FD,
	Tag = 0xF02B,
	ThLarge = 0xF009,
	ThermometerHalf = 0xF2C9,
	Thumbtack = 0xF08D,
	Times = 0xF00D,
	TimesCircle = 0xF057,
	TrashAlt = 0xF2ED,
	Undo = 0xF0E2,
	User = 0xF007,
	Users = 0xF0C0,
	Wind = 0xF72E,
}

public static class M3Icons
{
	private static ImFontPtr _font;
	private static bool _tried;

	public static ImFontPtr Font
	{
		get
		{
			if (!_tried)
			{
				_tried = true;
				_font = Load();
			}

			return _font.IsNull() ? ImGui.GetFont() : _font;
		}
	}

	public static string ToIconString(this FontAwesomeIcon icon)
	{
		return icon == FontAwesomeIcon.None ? string.Empty : char.ConvertFromUtf32((int)icon);
	}

	private static unsafe ImFontPtr Load()
	{
		using var stream = typeof(M3Icons).Assembly.GetManifestResourceStream("RebornMaterial.Fonts.FontAwesomeFreeSolid.otf");
		if (stream == null)
		{
			return default;
		}

		var length = (int)stream.Length;
		var data = ImGui.MemAlloc((nuint)length);
		stream.ReadExactly(new Span<byte>((void*)data, length));

		var config = new ImFontConfig
		{
			FontDataOwnedByAtlas = true,
			GlyphMaxAdvanceX = float.MaxValue,
			RasterizerMultiply = 1f,
			RasterizerDensity = 1f,
			PixelSnapH = true,
		};

		return ImGui.GetIO().Fonts.AddFontFromMemoryTTF(data, length, 16f, new ImFontConfigPtr(&config));
	}
}

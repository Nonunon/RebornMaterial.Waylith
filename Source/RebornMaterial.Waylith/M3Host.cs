using Waylith.Input;

namespace RebornMaterial;

public static class M3Host
{
	private static readonly Dictionary<string, string> Renamed = new()
	{
		["Backquote"] = "GraveAccent",
		["Quote"] = "Apostrophe",
		["Equals"] = "Equal",
		["LeftMeta"] = "LeftSuper",
		["RightMeta"] = "RightSuper",
		["ContextMenu"] = "Menu",
		["NumpadEnter"] = "KeypadEnter",
		["NumpadDivide"] = "KeypadDivide",
		["NumpadMultiply"] = "KeypadMultiply",
		["NumpadPlus"] = "KeypadAdd",
		["NumpadMinus"] = "KeypadSubtract",
		["NumpadPeriod"] = "KeypadDecimal",
		["NumpadEquals"] = "KeypadEqual",
	};

	private static readonly Queue<char> _typed = new();
	private static (Key Key, ImGuiKey ImKey)[]? _keys;
	private static bool[] _down = [];
	private static bool _ctrl;
	private static bool _shift;
	private static bool _alt;
	private static bool _super;
	private static bool _hooked;
	private static int _fedFrame = -1;
	private static bool _focused = true;
	private static bool _layersLooked;
	private static Func<bool>? _layersAvailable;
	private static Action<Verse.Window>? _layersClaim;
	private static Action<Verse.Window, bool>? _layersClaimRising;

	public static int MappedKeyCount => Keys.Length;

	public static string ClipboardSource { get; private set; } = "None";

	public static void FeedInput()
	{
		var frame = ImGui.GetFrameCount();
		if (frame == _fedFrame)
		{
			return;
		}

		_fedFrame = frame;
		Hook();

		var io = ImGui.GetIO();
		var focused = Waylith.EngineWindow.IsFocused;
		if (focused != _focused)
		{
			_focused = focused;
			io.AddFocusEvent(focused);
			if (!focused)
			{
				Array.Clear(_down);
				_ctrl = _shift = _alt = _super = false;
			}
		}

		var mouse = InputState.MousePosition;
		io.AddMousePosEvent(mouse.X, mouse.Y);
		io.AddMouseButtonEvent(0, InputState.IsMouseButtonDown(MouseButton.Left));
		io.AddMouseButtonEvent(1, InputState.IsMouseButtonDown(MouseButton.Right));
		io.AddMouseButtonEvent(2, InputState.IsMouseButtonDown(MouseButton.Middle));
		var scroll = InputState.ScrollDelta;
		if (io.WantCaptureMouse && (scroll.X != 0f || scroll.Y != 0f))
		{
			io.AddMouseWheelEvent(scroll.X, scroll.Y);
		}

		Modifier(io, ImGuiKey.Mod_Ctrl, InputState.CtrlDown, ref _ctrl);
		Modifier(io, ImGuiKey.Mod_Shift, InputState.ShiftDown, ref _shift);
		Modifier(io, ImGuiKey.Mod_Alt, InputState.AltDown, ref _alt);
		Modifier(io, ImGuiKey.Mod_Super, InputState.MetaDown, ref _super);

		var keys = Keys;
		for (var i = 0; i < keys.Length; i++)
		{
			var down = InputState.IsKeyDown(keys[i].Key);
			if (down != _down[i])
			{
				_down[i] = down;
				io.AddKeyEvent(keys[i].ImKey, down);
			}
		}

		lock (_typed)
		{
			while (_typed.Count > 0)
			{
				io.AddInputCharacterUTF16(_typed.Dequeue());
			}
		}
	}

	public static bool IsCovered(Verse.Window window)
	{
		var stack = Verse.Find.WindowStack;
		if (stack == null)
		{
			return false;
		}

		var found = false;
		for (var i = 0; i < stack.Count; i++)
		{
			var other = stack[i];
			if (ReferenceEquals(other, window))
			{
				found = true;
				continue;
			}

			if (found && (other.absorbInputAroundWindow || other.windowRect.Overlaps(window.windowRect)))
			{
				return true;
			}
		}

		return false;
	}

	public static bool Layered
	{
		get
		{
			FindUnderlayer();
			return _layersAvailable?.Invoke() == true;
		}
	}

	public static void Claim(Verse.Window window) => Claim(window, false);

	public static void Claim(Verse.Window window, bool riseAboveDialogs)
	{
		if (!Layered)
		{
			return;
		}

		if (_layersClaimRising != null)
		{
			_layersClaimRising(window, riseAboveDialogs);
		}
		else
		{
			_layersClaim!(window);
		}
	}

	public static bool ShouldHide(Verse.Window window) => !Layered && IsCovered(window);

	private static void FindUnderlayer()
	{
		if (_layersLooked)
		{
			return;
		}

		_layersLooked = true;
		try
		{
			var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "Underlayer");
			var layers = assembly?.GetType("Underlayer.Layers");
			var available = layers?.GetProperty("Available")?.GetMethod;
			var claim = layers?.GetMethod("Claim", [typeof(Verse.Window)]);
			if (available != null && claim != null)
			{
				_layersAvailable = available.CreateDelegate<Func<bool>>();
				_layersClaim = claim.CreateDelegate<Action<Verse.Window>>();
				_layersClaimRising = layers!.GetMethod("Claim", [typeof(Verse.Window), typeof(bool)])?.CreateDelegate<Action<Verse.Window, bool>>();
			}
		}
		catch (Exception e)
		{
			Verse.Log.Warning("[RebornMaterial] Underlayer found but not usable: " + e.Message);
		}
	}

	public static void ReleaseMouse()
	{
		ImGui.GetIO().AddMousePosEvent(-float.MaxValue, -float.MaxValue);
	}

	public static Key ToGameKey(ImGuiKey key)
	{
		foreach (var pair in Keys)
		{
			if (pair.ImKey == key)
			{
				return pair.Key;
			}
		}

		return Key.None;
	}

	public static string KeyName(Key key)
	{
		var name = key.ToString();
		if (name.StartsWith("Digit", StringComparison.Ordinal))
		{
			return name[5..];
		}

		if (name.StartsWith("Numpad", StringComparison.Ordinal))
		{
			return "Num " + name[6..];
		}

		return name switch
		{
			"Backquote" => "`",
			"Quote" => "'",
			"Semicolon" => ";",
			"Comma" => ",",
			"Period" => ".",
			"Slash" => "/",
			"Backslash" => "\\",
			"LeftBracket" => "[",
			"RightBracket" => "]",
			"Minus" => "-",
			"Equals" => "=",
			"LeftArrow" => "Left",
			"RightArrow" => "Right",
			"UpArrow" => "Up",
			"DownArrow" => "Down",
			"PageUp" => "Page Up",
			"PageDown" => "Page Down",
			_ => name,
		};
	}

	private static (Key Key, ImGuiKey ImKey)[] Keys
	{
		get
		{
			if (_keys != null)
			{
				return _keys;
			}

			var list = new List<(Key, ImGuiKey)>();
			foreach (var key in Enum.GetValues<Key>())
			{
				var name = key.ToString();
				if (Renamed.TryGetValue(name, out var renamed))
				{
					name = renamed;
				}
				else if (name.StartsWith("Digit", StringComparison.Ordinal))
				{
					name = "_" + name[5..];
				}
				else if (name.StartsWith("Numpad", StringComparison.Ordinal))
				{
					name = "Keypad" + name[6..];
				}

				if (key != Key.None && Enum.TryParse<ImGuiKey>(name, out var imKey) && imKey != ImGuiKey.None)
				{
					list.Add((key, imKey));
				}
			}

			_keys = [.. list];
			_down = new bool[_keys.Length];
			return _keys;
		}
	}

	private static void Modifier(ImGuiIOPtr io, ImGuiKey key, bool down, ref bool last)
	{
		if (down != last)
		{
			last = down;
			io.AddKeyEvent(key, down);
		}
	}

	private static void Hook()
	{
		if (_hooked)
		{
			return;
		}

		_hooked = true;
		ClipboardSource = M3Clipboard.Install();
		InputState.TextInput += c =>
		{
			lock (_typed)
			{
				if (_typed.Count < 256)
				{
					_typed.Enqueue(c);
				}
			}
		};
	}
}

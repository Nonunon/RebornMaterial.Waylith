namespace RebornMaterial;

public static class M3Draw
{
	private const int RailFadeBands = 6;

	public static void Container(ImDrawListPtr drawList, float2 min, float2 max, float4 fill, float rounding, float4? outline = null, float outlineSize = 1f)
	{
		if (fill.W > 0f)
		{
			drawList.AddRectFilled(min, max, M3.U32(fill), rounding);
		}

		if (outline is { } stroke && stroke.W > 0f)
		{
			drawList.AddRect(min, max, M3.U32(stroke), rounding, ImDrawFlags.None, outlineSize * M3.Scale);
		}
	}

	public static void Elevation(ImDrawListPtr drawList, float2 min, float2 max, float rounding, int level)
	{
		if (level <= 0)
		{
			return;
		}

		var scale = M3.Scale;
		var shadow = M3.Scheme.Shadow;
		var spread = level * 2f * scale;
		var offset = new float2(0f, level * 0.75f * scale);
		const int steps = 4;

		for (var i = steps; i >= 1; i--)
		{
			var t = (float)i / steps;
			var grow = spread * t;
			var alpha = 0.16f * (1f - t) * (1f - t);
			drawList.AddRectFilled(
				min - new float2(grow, grow) + offset,
				max + new float2(grow, grow) + offset,
				M3.U32(shadow, alpha),
				rounding + grow);
		}
	}

	public static void AccentRail(ImDrawListPtr drawList, float x, float width, float top, float bottom, float4 color, float fadeLength)
	{
		var height = bottom - top;
		if (width <= 0f || height <= 0f)
		{
			return;
		}

		var right = x + width;
		var fade = Math.Clamp(fadeLength, 0f, height * 0.5f);
		if (fade <= 0f)
		{
			drawList.AddRectFilled(new float2(x, top), new float2(right, bottom), M3.U32(color), width * 0.5f);
			return;
		}

		var solidTop = top + fade;
		var solidBottom = bottom - fade;
		if (solidBottom > solidTop)
		{
			drawList.AddRectFilled(new float2(x, solidTop), new float2(right, solidBottom), M3.U32(color));
		}

		for (var band = 0; band < RailFadeBands; band++)
		{
			var near = (float)band / RailFadeBands;
			var far = (float)(band + 1) / RailFadeBands;
			var nearColor = M3.U32(color, color.W * SmoothFade(near));
			var farColor = M3.U32(color, color.W * SmoothFade(far));

			drawList.AddRectFilledMultiColor(
				new float2(x, solidTop - (fade * far)),
				new float2(right, solidTop - (fade * near)),
				farColor, farColor, nearColor, nearColor);

			drawList.AddRectFilledMultiColor(
				new float2(x, solidBottom + (fade * near)),
				new float2(right, solidBottom + (fade * far)),
				nearColor, nearColor, farColor, farColor);
		}
	}

	public static float ResolveFade(float fadeLength, float fraction, float railHeight)
	{
		return MathF.Max(0f, MathF.Min(fadeLength * M3.Scale, railHeight * fraction));
	}

	public static void Arc(ImDrawListPtr drawList, float2 center, float radius, float from, float to, float4 color, float thickness)
	{
		if (to - from <= 0.001f)
		{
			return;
		}

		const float top = -MathF.PI * 0.5f;
		var segments = Math.Max(4, (int)MathF.Ceiling(48f * (to - from)));
		drawList.PathArcTo(center, radius, top + (MathF.Tau * from), top + (MathF.Tau * to), segments);
		drawList.PathStroke(M3.U32(color), ImDrawFlags.None, thickness);
	}

	public static void VerticalGradient(ImDrawListPtr drawList, float2 min, float2 max, float4 top, float4 bottom)
	{
		var topColor = M3.U32(top);
		var bottomColor = M3.U32(bottom);
		drawList.AddRectFilledMultiColor(min, max, topColor, topColor, bottomColor, bottomColor);
	}

	public static float2 MeasureIcon(FontAwesomeIcon icon)
	{
		using var font = ImRaii.PushFont(M3Icons.Font);
		return ImGui.CalcTextSize(icon.ToIconString());
	}

	public static void Icon(ImDrawListPtr drawList, FontAwesomeIcon icon, float2 position, float4 color)
	{
		using var font = ImRaii.PushFont(M3Icons.Font);
		drawList.AddText(M3Icons.Font, ImGui.GetFontSize(), position, M3.U32(color), icon.ToIconString());
	}

	public static void IconCentered(ImDrawListPtr drawList, FontAwesomeIcon icon, float2 min, float2 max, float4 color, float scale = 1f)
	{
		using var font = ImRaii.PushFont(M3Icons.Font);
		var text = icon.ToIconString();
		var size = ImGui.CalcTextSize(text) * scale;
		var position = min + (((max - min) - size) * 0.5f);
		drawList.AddText(M3Icons.Font, ImGui.GetFontSize() * scale, position, M3.U32(color), text);
	}

	// Outlines the last item while the keyboard or a gamepad has it focused. Mouse users never see it.
	public static void FocusRing(float2 min, float2 max, float rounding)
	{
		if (!ImGui.IsItemFocused() || !ImGui.GetIO().NavVisible)
		{
			return;
		}

		var scale = M3.Scale;
		var gap = 2f * scale;
		ImGui.GetWindowDrawList().AddRect(min - new float2(gap), max + new float2(gap), M3.U32(M3.Scheme.Secondary),
			rounding + gap, ImDrawFlags.None, 2.5f * scale);
	}

	public static bool Texture(ImDrawListPtr drawList, IM3Texture? texture, float2 min, float size, float alpha = 1f)
	{
		return M3ActionIcon.Image(drawList, texture, min, min + new float2(size, size), M3.ShapeExtraSmall, alpha);
	}

	public static float WrappedText(string text, float2 position, float wrapWidth, float4 color)
	{
		ImGui.SetCursorScreenPos(position);
		using (ImRaii.PushColor(ImGuiCol.Text, color))
		{
			ImGui.PushTextWrapPos(ImGui.GetCursorPosX() + wrapWidth);
			ImGui.Text(text);
			ImGui.PopTextWrapPos();
		}

		return ImGui.GetItemRectMax().Y;
	}

	private static float SmoothFade(float t)
	{
		return 1f - (t * t * (3f - (2f * t)));
	}
}

public static class M3Motion
{
	private const int MaxTrackedValues = 512;

	private static readonly Dictionary<uint, float> _values = [];

	public const float FastDuration = 0.14f;
	public const float EmphasisedDuration = 0.28f;

	public static float Approach(uint id, float target, float duration)
	{
		if (!_values.TryGetValue(id, out var current))
		{
			if (_values.Count >= MaxTrackedValues)
			{
				_values.Clear();
			}

			_values[id] = target;
			return target;
		}

		if (MathF.Abs(target - current) < 0.001f)
		{
			_values[id] = target;
			return target;
		}

		var delta = ImGui.GetIO().DeltaTime;
		var step = duration <= 0f ? 1f : 1f - MathF.Exp(-delta / (duration * 0.35f));
		current = float.Lerp(current, target, Math.Clamp(step, 0f, 1f));
		_values[id] = current;
		return current;
	}

	public static float Approach(string id, float target, float duration)
	{
		return Approach(ImGui.GetID(id), target, duration);
	}

	public static void Reset()
	{
		_values.Clear();
	}
}

namespace RebornMaterial;

public static class ImRaii
{
	public static FontDisposable PushFont(M3Font font, bool condition = true)
	{
		if (!condition)
		{
			return default;
		}

		M3Fonts.Push(font);
		return new FontDisposable(true);
	}

	public static FontDisposable PushFont(ImFontPtr font, bool condition = true)
	{
		if (!condition)
		{
			return default;
		}

		ImGui.PushFont(font, 0f);
		return new FontDisposable(true);
	}

	public static ColorDisposable PushColor(ImGuiCol target, float4 color, bool condition = true)
	{
		return new ColorDisposable().Push(target, color, condition);
	}

	public static ColorDisposable PushColor(ImGuiCol target, ImColor8 color, bool condition = true)
	{
		return new ColorDisposable().Push(target, color, condition);
	}

	public static StyleDisposable PushStyle(ImGuiStyleVar target, float value, bool condition = true)
	{
		return new StyleDisposable().Push(target, value, condition);
	}

	public static StyleDisposable PushStyle(ImGuiStyleVar target, float2 value, bool condition = true)
	{
		return new StyleDisposable().Push(target, value, condition);
	}

	public static IdDisposable PushId(string id)
	{
		ImGui.PushID(id);
		return new IdDisposable(true);
	}

	public static IdDisposable PushId(int id)
	{
		ImGui.PushID(id);
		return new IdDisposable(true);
	}

	public static EndDisposable Disabled(bool disabled = true)
	{
		ImGui.BeginDisabled(disabled);
		return new EndDisposable(true, true, ImGui.EndDisabled);
	}

	public static EndDisposable TextWrapPos(float position)
	{
		ImGui.PushTextWrapPos(position);
		return new EndDisposable(true, true, ImGui.PopTextWrapPos);
	}

	public static EndDisposable Tooltip()
	{
		var open = ImGui.BeginTooltip();
		return new EndDisposable(open, open, ImGui.EndTooltip);
	}

	public static EndDisposable Popup(string id, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
	{
		var open = ImGui.BeginPopup(id, flags);
		return new EndDisposable(open, open, ImGui.EndPopup);
	}

	public static EndDisposable Group()
	{
		ImGui.BeginGroup();
		return new EndDisposable(true, true, ImGui.EndGroup);
	}

	public static EndDisposable Table(string id, int columns, ImGuiTableFlags flags = ImGuiTableFlags.None, float2 outerSize = default, float innerWidth = 0f)
	{
		var open = ImGui.BeginTable(id, columns, flags, outerSize, innerWidth);
		return new EndDisposable(open, open, ImGui.EndTable);
	}

	public static EndDisposable Child(string id, float2 size, bool border = false, ImGuiWindowFlags flags = ImGuiWindowFlags.None)
	{
		var open = ImGui.BeginChild(id, size, border ? ImGuiChildFlags.Borders : ImGuiChildFlags.None, flags);
		return new EndDisposable(open, true, ImGui.EndChild);
	}

	public readonly struct FontDisposable(bool pushed) : IDisposable
	{
		public void Dispose()
		{
			if (pushed)
			{
				ImGui.PopFont();
			}
		}
	}

	public sealed class ColorDisposable : IDisposable
	{
		private int _count;

		public ColorDisposable Push(ImGuiCol target, float4 color, bool condition = true)
		{
			if (condition)
			{
				ImGui.PushStyleColor(target, color);
				_count++;
			}

			return this;
		}

		public ColorDisposable Push(ImGuiCol target, ImColor8 color, bool condition = true)
		{
			if (condition)
			{
				ImGui.PushStyleColor(target, color);
				_count++;
			}

			return this;
		}

		public void Dispose()
		{
			if (_count > 0)
			{
				ImGui.PopStyleColor(_count);
				_count = 0;
			}
		}
	}

	public sealed class StyleDisposable : IDisposable
	{
		private int _count;

		public StyleDisposable Push(ImGuiStyleVar target, float value, bool condition = true)
		{
			if (condition)
			{
				ImGui.PushStyleVar(target, value);
				_count++;
			}

			return this;
		}

		public StyleDisposable Push(ImGuiStyleVar target, float2 value, bool condition = true)
		{
			if (condition)
			{
				ImGui.PushStyleVar(target, value);
				_count++;
			}

			return this;
		}

		public void Dispose()
		{
			if (_count > 0)
			{
				ImGui.PopStyleVar(_count);
				_count = 0;
			}
		}
	}

	public readonly struct IdDisposable(bool pushed) : IDisposable
	{
		public void Dispose()
		{
			if (pushed)
			{
				ImGui.PopID();
			}
		}
	}

	public readonly struct EndDisposable(bool success, bool end, Action endAction) : IDisposable
	{
		public bool Success => success;

		public static implicit operator bool(EndDisposable value)
		{
			return value.Success;
		}

		public void Dispose()
		{
			if (end)
			{
				endAction();
			}
		}
	}
}

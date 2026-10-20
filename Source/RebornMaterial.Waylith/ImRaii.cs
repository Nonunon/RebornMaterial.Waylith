namespace RebornMaterial;

public static class ImRaii
{
	public static FontDisposable PushFont(M3Font font)
	{
		M3Fonts.Push(font);
		return new FontDisposable(true);
	}

	public static ColorDisposable PushColor(ImGuiCol target, float4 color, bool condition = true)
	{
		if (!condition)
		{
			return default;
		}

		ImGui.PushStyleColor(target, color);
		return new ColorDisposable(1);
	}

	public static ColorDisposable PushColor(ImGuiCol target, ImColor8 color, bool condition = true)
	{
		if (!condition)
		{
			return default;
		}

		ImGui.PushStyleColor(target, color);
		return new ColorDisposable(1);
	}

	public static StyleDisposable PushStyle(ImGuiStyleVar target, float value, bool condition = true)
	{
		if (!condition)
		{
			return default;
		}

		ImGui.PushStyleVar(target, value);
		return new StyleDisposable(1);
	}

	public static StyleDisposable PushStyle(ImGuiStyleVar target, float2 value, bool condition = true)
	{
		if (!condition)
		{
			return default;
		}

		ImGui.PushStyleVar(target, value);
		return new StyleDisposable(1);
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

	public readonly struct ColorDisposable(int count) : IDisposable
	{
		public void Dispose()
		{
			if (count > 0)
			{
				ImGui.PopStyleColor(count);
			}
		}
	}

	public readonly struct StyleDisposable(int count) : IDisposable
	{
		public void Dispose()
		{
			if (count > 0)
			{
				ImGui.PopStyleVar(count);
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
}

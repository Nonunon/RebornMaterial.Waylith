using System.Text;

namespace RebornMaterial;

internal static class M3Input
{
	private static ImInputString? _buffer;

	public static bool Text(string label, ref string text, int maxLength, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
	{
		var buffer = Prepare(text, maxLength);
		if (!ImGui.InputText(label, buffer, flags))
		{
			return false;
		}

		text = buffer.ToString();
		return true;
	}

	public static bool TextWithHint(string label, string hint, ref string text, int maxLength, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
	{
		var buffer = Prepare(text, maxLength);
		if (!ImGui.InputTextWithHint(label, hint, buffer, flags))
		{
			return false;
		}

		text = buffer.ToString();
		return true;
	}

	private static ImInputString Prepare(string text, int maxLength)
	{
		var capacity = Math.Max(maxLength, Encoding.UTF8.GetByteCount(text)) + 1;
		if (_buffer == null || _buffer.Capacity != capacity)
		{
			_buffer = new ImInputString(capacity);
		}

		_buffer.SetValue(text.AsSpan());
		return _buffer;
	}
}

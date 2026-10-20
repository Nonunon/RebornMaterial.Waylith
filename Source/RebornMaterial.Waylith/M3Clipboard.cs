using System.Runtime.InteropServices;
using System.Text;

namespace RebornMaterial;

internal static unsafe class M3Clipboard
{
	private static nint _buffer;

	public static string Install()
	{
		var platform = ImGui.GetPlatformIO();
		if (platform.Platform_GetClipboardTextFn != 0)
		{
			return "ImGui";
		}

		platform.Platform_GetClipboardTextFn = (nint)(delegate* unmanaged<nint, byte*>)&Get;
		platform.Platform_SetClipboardTextFn = (nint)(delegate* unmanaged<nint, byte*, void>)&Set;
		return "Waylith";
	}

	[UnmanagedCallersOnly]
	private static byte* Get(nint context)
	{
		try
		{
			var bytes = Encoding.UTF8.GetBytes(Waylith.Input.Clipboard.Text);
			if (_buffer != 0)
			{
				Marshal.FreeHGlobal(_buffer);
			}

			_buffer = Marshal.AllocHGlobal(bytes.Length + 1);
			Marshal.Copy(bytes, 0, _buffer, bytes.Length);
			((byte*)_buffer)[bytes.Length] = 0;
			return (byte*)_buffer;
		}
		catch (Exception)
		{
			return null;
		}
	}

	[UnmanagedCallersOnly]
	private static void Set(nint context, byte* text)
	{
		try
		{
			Waylith.Input.Clipboard.Text = Marshal.PtrToStringUTF8((nint)text) ?? string.Empty;
		}
		catch (Exception)
		{
		}
	}
}

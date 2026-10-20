using System.Reflection;
using UnityStubs.Gui;

namespace RebornMaterial;

public sealed class M3GameTexture(UnityEngine.Texture? texture) : IM3Texture
{
	private delegate bool Resolver(UnityEngine.Texture? texture, out ImTextureRef handle);

	private static Resolver? _resolver;
	private static object? _backend;
	private static bool _missing;

	private int _frame = -1;
	private bool _valid;
	private ImTextureRef _handle;

	public UnityEngine.Texture? Texture { get; set; } = texture;

	public ImTextureRef Handle
	{
		get
		{
			Refresh();
			return _handle;
		}
	}

	public bool IsValid
	{
		get
		{
			Refresh();
			return _valid;
		}
	}

	public int Width => Texture?.width ?? 0;

	public int Height => Texture?.height ?? 0;

	public static bool Supported => GetResolver() != null;

	public static bool TryResolve(UnityEngine.Texture? texture, out ImTextureRef handle)
	{
		handle = default;
		var resolver = GetResolver();
		return texture != null && resolver != null && resolver(texture, out handle);
	}

	private void Refresh()
	{
		var frame = ImGui.GetFrameCount();
		if (frame == _frame)
		{
			return;
		}

		_frame = frame;
		_valid = TryResolve(Texture, out _handle);
	}

	private static Resolver? GetResolver()
	{
		var backend = GuiBackend.Current;
		if (backend == null || _missing)
		{
			return null;
		}

		if (ReferenceEquals(backend, _backend) && _resolver != null)
		{
			return _resolver;
		}

		var method = backend.GetType().GetMethod("TryResolveTexture", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
			null, [typeof(UnityEngine.Texture), typeof(ImTextureRef).MakeByRefType()], null);
		if (method == null || method.ReturnType != typeof(bool))
		{
			_missing = true;
			Verse.Log.Warning("[RebornMaterial] This RimWorld build has no TryResolveTexture on its GUI backend, so game textures will not draw.");
			return null;
		}

		_backend = backend;
		_resolver = (Resolver)Delegate.CreateDelegate(typeof(Resolver), backend, method);
		return _resolver;
	}
}

namespace RebornMaterial;

public interface IM3Texture
{
	ImTextureRef Handle { get; }

	bool IsValid { get; }

	int Width { get; }

	int Height { get; }
}

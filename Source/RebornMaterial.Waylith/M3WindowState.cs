namespace RebornMaterial;

public struct WindowSizeConstraints
{
	public float2 MinimumSize;

	public float2 MaximumSize;
}

public sealed class M3WindowState
{
	public float2? Position { get; set; }

	public ImGuiCond PositionCondition { get; set; } = ImGuiCond.FirstUseEver;

	public float2? Size { get; set; }

	public ImGuiCond SizeCondition { get; set; } = ImGuiCond.FirstUseEver;

	public WindowSizeConstraints? SizeConstraints { get; set; }

	public ImGuiWindowFlags Flags { get; set; }

	public void Apply()
	{
		if (Position is { } position)
		{
			ImGui.SetNextWindowPos(position, PositionCondition);
		}

		if (Size is { } size)
		{
			ImGui.SetNextWindowSize(size, SizeCondition);
		}

		if (SizeConstraints is { } constraints)
		{
			ImGui.SetNextWindowSizeConstraints(constraints.MinimumSize, constraints.MaximumSize);
		}
	}
}

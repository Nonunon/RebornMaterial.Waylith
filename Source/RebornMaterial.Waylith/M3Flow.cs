namespace RebornMaterial;

// Make one at the start of a line, then call Next with each item's width just before drawing it.
public struct M3Flow
{
	private readonly float _gap;
	private readonly float _available;
	private float _used;

	public M3Flow()
		: this(null, null)
	{
	}

	public M3Flow(float? gap = null, float? width = null)
	{
		_gap = gap ?? M3.Space2;
		_available = width ?? ImGui.GetContentRegionAvail().X;
	}

	public void Next(float width)
	{
		if (_used > 0f && _used + _gap + width <= _available)
		{
			ImGui.SameLine(0f, _gap);
			_used += _gap + width;
		}
		else
		{
			_used = width;
		}
	}
}

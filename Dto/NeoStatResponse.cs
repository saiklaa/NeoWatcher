namespace NeoWatcher.Dto;

public sealed record NeoStatResponse
{
	public DateTime Date { get; init; }

	public int ObjectCount { get; init; }

	public double MaxDiameter { get; init; }

	public double AvgVelocity { get; init; }

	public bool HasHazardousObjects { get; init; }
}
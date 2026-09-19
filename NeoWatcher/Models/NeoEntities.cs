namespace NeoWatcher.Models;

public sealed class Asteroid
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public double EstimatedDiameterMin { get; set; }

    public double EstimatedDiameterMax { get; set; }

    public bool IsPotentiallyHazardous { get; set; }

    public ICollection<CloseApproach> CloseApproaches { get; set; } = new List<CloseApproach>();
}

public sealed class CloseApproach
{
    public int Id { get; set; }

    public string AsteroidId { get; set; } = string.Empty;

    public DateTime CloseApproachDate { get; set; }

    public double RelativeVelocityKmh { get; set; }

    public double MissDistanceKm { get; set; }

    public Asteroid Asteroid { get; set; } = null!;
}
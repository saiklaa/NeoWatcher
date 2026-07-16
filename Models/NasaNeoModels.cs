using System.Text.Json.Serialization;

public sealed class NeoFeedResponse
{
    [JsonPropertyName("near_earth_objects")]
    public Dictionary<string, List<NeoObject>> NearEarthObjects { get; set; } = new();
}

public sealed class NeoObject
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("is_potentially_hazardous_asteroid")]
    public bool IsHazardous { get; set; }

    [JsonPropertyName("estimated_diameter")]
    public DiameterWrapper EstimatedDiameter { get; set; } = new();

    [JsonPropertyName("close_approach_data")]
    public List<CloseApproachData> CloseApproachData { get; set; } = new();
}

public sealed class DiameterWrapper
{
    [JsonPropertyName("meters")]
    public DiameterMeters Meters { get; set; } = new();
}

public sealed class DiameterMeters
{
    [JsonPropertyName("estimated_diameter_min")]
    public double EstimatedDiameterMin { get; set; }

    [JsonPropertyName("estimated_diameter_max")]
    public double EstimatedDiameterMax { get; set; }
}

public sealed class CloseApproachData
{
    [JsonPropertyName("close_approach_date")]
    public string CloseApproachDate { get; set; } = string.Empty;

    [JsonPropertyName("relative_velocity")]
    public RelativeVelocity RelativeVelocity { get; set; } = new();

    [JsonPropertyName("miss_distance")]
    public MissDistance MissDistance { get; set; } = new();
}

public sealed class RelativeVelocity
{
    [JsonPropertyName("kilometers_per_hour")]
    public string KmH { get; set; } = string.Empty;
}

public sealed class MissDistance
{
    [JsonPropertyName("kilometers")]
    public string Kilometers { get; set; } = string.Empty;
}

public sealed class NearEarthObject
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public DateTime CloseApproachDate { get; set; }

    public double EstimatedDiameterMin { get; set; }

    public double EstimatedDiameterMax { get; set; }

    public bool IsPotentiallyHazardous { get; set; }

    public double RelativeVelocityKmh { get; set; }

    public double MissDistanceKm { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
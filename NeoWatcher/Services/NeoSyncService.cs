using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NeoWatcher.Models;

public class NeoSyncService
{
    private readonly HttpClient _client;
    private readonly NeoContext _context;
    private readonly ILogger<NeoSyncService> _logger;
    private readonly IConfiguration _config;

    public NeoSyncService(HttpClient client, NeoContext context, ILogger<NeoSyncService> logger, IConfiguration config)
    {
        _client = client;
        _context = context;
        _logger = logger;
        _config = config;
    }

    public async Task FetchAndSyncAsync(CancellationToken cancellationToken = default)
    {
        var start = DateTime.UtcNow.Date.AddDays(-3);
        var end = DateTime.UtcNow.Date;

        var apiKey = _config["Nasa:ApiKey"] ?? "DEMO_KEY";
        var url = $"https://api.nasa.gov/neo/rest/v1/feed?start_date={start:yyyy-MM-dd}&end_date={end:yyyy-MM-dd}&api_key={apiKey}";
        var response = await _client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var feed = JsonSerializer.Deserialize<NeoFeedResponse>(json);

        if (feed?.NearEarthObjects is null)
        {
            _logger.LogWarning("NASA feed returned no near earth objects.");
            return;
        }

        var insertedCount = 0;
        var updatedCount = 0;

        var incomingIds = feed.NearEarthObjects.SelectMany(d => d.Value).Select(o => o.Id).Distinct().ToList();
        var existingAsteroids = await _context.Asteroids
            .Where(x => incomingIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);
        var existingApproaches = await _context.CloseApproaches
            .Where(x => incomingIds.Contains(x.AsteroidId))
            .ToDictionaryAsync(x => (x.AsteroidId, x.CloseApproachDate), cancellationToken);

        foreach (var day in feed.NearEarthObjects)
        {
            foreach (var obj in day.Value)
            {
                if (!existingAsteroids.TryGetValue(obj.Id, out var asteroid))
                {
                    asteroid = new Asteroid { Id = obj.Id };
                    _context.Asteroids.Add(asteroid);
                    existingAsteroids.Add(obj.Id, asteroid);
                }

                asteroid.Name = obj.Name;
                asteroid.EstimatedDiameterMin = obj.EstimatedDiameter.Meters.EstimatedDiameterMin;
                asteroid.EstimatedDiameterMax = obj.EstimatedDiameter.Meters.EstimatedDiameterMax;
                asteroid.IsPotentiallyHazardous = obj.IsHazardous;

                foreach (var approach in obj.CloseApproachData)
                {
                    if (!DateTime.TryParse(
                        approach.CloseApproachDate,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out var closeApproachDate))
                    {
                        continue;
                    }

                    var relativeVelocity = double.TryParse(approach.RelativeVelocity.KmH, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedVelocity)
                        ? parsedVelocity
                        : 0;

                    var missDistance = double.TryParse(approach.MissDistance.Kilometers, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedDistance)
                        ? parsedDistance
                        : 0;

                    var approachKey = (obj.Id, closeApproachDate);
                    if (existingApproaches.TryGetValue(approachKey, out var existing))
                    {
                        existing.CloseApproachDate = closeApproachDate;
                        existing.RelativeVelocityKmh = relativeVelocity;
                        existing.MissDistanceKm = missDistance;
                        updatedCount++;
                        continue;
                    }

                    var closeApproach = new CloseApproach
                    {
                        AsteroidId = obj.Id,
                        CloseApproachDate = closeApproachDate,
                        RelativeVelocityKmh = relativeVelocity,
                        MissDistanceKm = missDistance
                    };

                    _context.CloseApproaches.Add(closeApproach);
                    existingApproaches.Add(approachKey, closeApproach);
                    insertedCount++;
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("NASA NEO sync complete. Inserted: {InsertedCount}, Updated: {UpdatedCount}", insertedCount, updatedCount);
    }
}
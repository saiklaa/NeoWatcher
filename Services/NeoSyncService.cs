using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

public class NeoSyncService
{
    private readonly HttpClient _client;
    private readonly NeoContext _context;
    private readonly ILogger<NeoSyncService> _logger;

    public NeoSyncService(HttpClient client, NeoContext context, ILogger<NeoSyncService> logger)
    {
        _client = client;
        _context = context;
        _logger = logger;
    }

    public async Task FetchAndSyncAsync(CancellationToken cancellationToken = default)
    {
        var start = DateTime.UtcNow.Date.AddDays(-3);
        var end = DateTime.UtcNow.Date;

        var url = $"https://api.nasa.gov/neo/rest/v1/feed?start_date={start:yyyy-MM-dd}&end_date={end:yyyy-MM-dd}&api_key=DEMO_KEY";
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

        foreach (var day in feed.NearEarthObjects)
        {
            foreach (var obj in day.Value)
            {
                var approach = obj.CloseApproachData.FirstOrDefault();
                if (approach is null)
                {
                    continue;
                }

                if (!DateTime.TryParse(approach.CloseApproachDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out var closeApproachDate))
                {
                    continue;
                }

                var existing = await _context.NearEarthObjects.FindAsync(new object[] { obj.Id }, cancellationToken);
                var relativeVelocity = double.TryParse(approach.RelativeVelocity.KmH, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedVelocity)
                    ? parsedVelocity
                    : 0;

                var missDistance = double.TryParse(approach.MissDistance.Kilometers, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsedDistance)
                    ? parsedDistance
                    : 0;

                if (existing is not null)
                {
                    existing.Name = obj.Name;
                    existing.CloseApproachDate = closeApproachDate;
                    existing.EstimatedDiameterMin = obj.EstimatedDiameter.Meters.EstimatedDiameterMin;
                    existing.EstimatedDiameterMax = obj.EstimatedDiameter.Meters.EstimatedDiameterMax;
                    existing.IsPotentiallyHazardous = obj.IsHazardous;
                    existing.RelativeVelocityKmh = relativeVelocity;
                    existing.MissDistanceKm = missDistance;
                    updatedCount++;
                    continue;
                }

                var neo = new NearEarthObject
                {
                    Id = obj.Id,
                    Name = obj.Name,
                    CloseApproachDate = closeApproachDate,
                    EstimatedDiameterMin = obj.EstimatedDiameter.Meters.EstimatedDiameterMin,
                    EstimatedDiameterMax = obj.EstimatedDiameter.Meters.EstimatedDiameterMax,
                    IsPotentiallyHazardous = obj.IsHazardous,
                    RelativeVelocityKmh = relativeVelocity,
                    MissDistanceKm = missDistance
                };

                _context.NearEarthObjects.Add(neo);
                insertedCount++;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("NASA NEO sync complete. Inserted: {InsertedCount}, Updated: {UpdatedCount}", insertedCount, updatedCount);
    }
}
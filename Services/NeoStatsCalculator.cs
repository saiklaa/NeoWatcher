using Microsoft.EntityFrameworkCore;
using NeoWatcher.Dto;
using NeoWatcher.Models;

namespace NeoWatcher.Services;

public class NeoStatsCalculator
{
    private readonly NeoContext _db;

    public NeoStatsCalculator(NeoContext db) => _db = db;

    public async Task<List<NeoStatResponse>> GetStatsAsync(NeoStatsQuery query, CancellationToken cancellationToken = default)
    {
        var neos = _db.NearEarthObjects.AsNoTracking();

        if (query.From.HasValue)
        {
            neos = neos.Where(x => x.CloseApproachDate >= query.From.Value.Date);
        }

        if (query.To.HasValue)
        {
            // include the whole 'to' day
            neos = neos.Where(x => x.CloseApproachDate < query.To.Value.Date.AddDays(1));
        }

        if (query.Hazardous.HasValue)
        {
            neos = neos.Where(x => x.IsPotentiallyHazardous == query.Hazardous.Value);
        }

        if (query.MinDiameter.HasValue)
        {
            neos = neos.Where(x => x.EstimatedDiameterMax >= query.MinDiameter.Value);
        }

        if (query.MaxDiameter.HasValue)
        {
            neos = neos.Where(x => x.EstimatedDiameterMin <= query.MaxDiameter.Value);
        }

        var stats = await neos
            .GroupBy(x => x.CloseApproachDate.Date)
            .Select(g => new NeoStatResponse
            {
                Date = g.Key,
                ObjectCount = g.Count(),
                MaxDiameter = g.Max(x => x.EstimatedDiameterMax),
                AvgVelocity = g.Average(x => x.RelativeVelocityKmh),
                HasHazardousObjects = g.Any(x => x.IsPotentiallyHazardous)
            })
            .ToListAsync(cancellationToken);

        // sort
        stats = NeoWatcher.Services.NeoStatsSorter.ApplySort(stats.Select(s => new NeoWatcher.Models.NeoStatViewModel {
            Date = s.Date,
            ObjectCount = s.ObjectCount,
            MaxDiameter = s.MaxDiameter,
            AvgVelocity = s.AvgVelocity,
            HasHazardousObjects = s.HasHazardousObjects
        }).ToList(), query.Sort.ToString().ToLowerInvariant(), query.Order.ToString().ToLowerInvariant())
            .Select(vm => new NeoStatResponse
            {
                Date = vm.Date,
                ObjectCount = vm.ObjectCount,
                MaxDiameter = vm.MaxDiameter,
                AvgVelocity = vm.AvgVelocity,
                HasHazardousObjects = vm.HasHazardousObjects
            })
            .ToList();

        return stats;
    }

    public async Task<List<NeoStatViewModel>> GetStatsForViewAsync(NeoFilterViewModel filter, CancellationToken cancellationToken = default)
    {
        // map filter to query
        var q = new NeoStatsQuery
        {
            From = filter.From,
            To = filter.To,
            Hazardous = filter.Hazardous,
            MinDiameter = filter.MinDiameter,
            MaxDiameter = filter.MaxDiameter,
            Sort = Enum.TryParse<NeoStatsSortBy>(filter.SortBy, true, out var sb) ? sb : NeoStatsSortBy.Date,
            Order = Enum.TryParse<NeoSortOrder>(filter.SortDir, true, out var so) ? so : NeoSortOrder.Asc
        };

        var stats = await GetStatsAsync(q, cancellationToken);

        return stats.Select(s => new NeoStatViewModel
        {
            Date = s.Date,
            ObjectCount = s.ObjectCount,
            MaxDiameter = s.MaxDiameter,
            AvgVelocity = s.AvgVelocity,
            HasHazardousObjects = s.HasHazardousObjects
        }).ToList();
    }
}

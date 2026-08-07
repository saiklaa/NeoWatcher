using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NeoWatcher.Dto;
using NeoWatcher.Models;

namespace NeoWatcher.Controllers;

[ApiController]
[Route("api/neo")]
public sealed class NeoStatsController : ControllerBase
{
	private readonly NeoContext _context;
	private readonly IMemoryCache _cache;

	public NeoStatsController(NeoContext context, IMemoryCache cache)
	{
		_context = context;
		_cache = cache;
	}

	[HttpGet("stats")]
	[ProducesResponseType(typeof(List<NeoStatResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<List<NeoStatResponse>>> GetStats([FromQuery] NeoStatsQuery query, CancellationToken cancellationToken)
	{
		var cacheKey = $"neo:stats:{query.BuildCacheKey()}";
		if (_cache.TryGetValue(cacheKey, out List<NeoStatResponse>? cached))
		{
			return Ok(cached);
		}

		var neos = _context.NearEarthObjects.AsNoTracking();

		if (query.From.HasValue)
		{
			neos = neos.Where(x => x.CloseApproachDate >= query.From.Value.Date);
		}

		if (query.To.HasValue)
		{
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

		var filtered = await neos.ToListAsync(cancellationToken);

		var stats = filtered
			.GroupBy(x => x.CloseApproachDate.Date)
			.Select(group => new NeoStatResponse
			{
				Date = group.Key,
				ObjectCount = group.Count(),
				MaxDiameter = group.Max(x => x.EstimatedDiameterMax),
				AvgVelocity = group.Average(x => x.RelativeVelocityKmh),
				HasHazardousObjects = group.Any(x => x.IsPotentiallyHazardous)
			})
			.ToList();

		stats = ApplySort(stats, query.Sort, query.Order);

		_cache.Set(cacheKey, stats, new MemoryCacheEntryOptions
		{
			AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
		});

		return Ok(stats);
	}

	private static List<NeoStatResponse> ApplySort(List<NeoStatResponse> stats, NeoStatsSortBy sort, NeoSortOrder order)
	{
		return (sort, order) switch
		{
			(NeoStatsSortBy.Date, NeoSortOrder.Desc) => stats.OrderByDescending(x => x.Date).ToList(),
			(NeoStatsSortBy.ObjectCount, NeoSortOrder.Desc) => stats.OrderByDescending(x => x.ObjectCount).ToList(),
			(NeoStatsSortBy.MaxDiameter, NeoSortOrder.Desc) => stats.OrderByDescending(x => x.MaxDiameter).ToList(),
			(NeoStatsSortBy.AvgVelocity, NeoSortOrder.Desc) => stats.OrderByDescending(x => x.AvgVelocity).ToList(),
			(NeoStatsSortBy.ObjectCount, _) => stats.OrderBy(x => x.ObjectCount).ToList(),
			(NeoStatsSortBy.MaxDiameter, _) => stats.OrderBy(x => x.MaxDiameter).ToList(),
			(NeoStatsSortBy.AvgVelocity, _) => stats.OrderBy(x => x.AvgVelocity).ToList(),
			_ => stats.OrderBy(x => x.Date).ToList()
		};
	}
}
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
	private readonly NeoStatsCalculator _calculator;
	private readonly IMemoryCache _cache;

	public NeoStatsController(NeoStatsCalculator calculator, IMemoryCache cache)
	{
		_calculator = calculator;
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

		var stats = await _calculator.GetStatsAsync(query, cancellationToken);

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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using NeoWatcher.Dto;
using NeoWatcher.Services;
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
}
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace NeoWatcher.Dto;

/// <summary>
/// The field to sort the stats by.
/// </summary>
public enum NeoStatsSortBy
{
	/// <summary>Sort by date of close approach (default).</summary>
	Date,

	/// <summary>Sort by number of objects in the group.</summary>
	ObjectCount,

	/// <summary>Sort by maximum estimated diameter in the group.</summary>
	MaxDiameter,

	/// <summary>Sort by average relative velocity in the group.</summary>
	AvgVelocity
}

/// <summary>
/// Sort direction for stats results.
/// </summary>
public enum NeoSortOrder
{
	/// <summary>Ascending order.</summary>
	Asc,

	/// <summary>Descending order.</summary>
	Desc
}

/// <summary>
/// Query parameters for GET /api/neo/stats.
/// </summary>
public sealed record NeoStatsQuery : IValidatableObject
{
	/// <summary>
	/// Lower bound for close approach date (inclusive). Format: yyyy-MM-dd. Example: 2026-07-01
	/// </summary>
	[FromQuery(Name = "from")]
	public DateTime? From { get; init; }

	/// <summary>
	/// Upper bound for close approach date (inclusive). Format: yyyy-MM-dd. Example: 2026-07-07
	/// </summary>
	[FromQuery(Name = "to")]
	public DateTime? To { get; init; }

	/// <summary>
	/// Filter by potentially hazardous flag. Example: true
	/// </summary>
	[FromQuery(Name = "hazardous")]
	public bool? Hazardous { get; init; }

	/// <summary>
	/// Minimum estimated diameter (meters). Example: 10.0
	/// </summary>
	[FromQuery(Name = "min_diameter")]
	public double? MinDiameter { get; init; }

	/// <summary>
	/// Maximum estimated diameter (meters). Example: 100.0
	/// </summary>
	[FromQuery(Name = "max_diameter")]
	public double? MaxDiameter { get; init; }

	/// <summary>
	/// Field to sort by. Allowed values: Date, ObjectCount, MaxDiameter, AvgVelocity. Default: Date
	/// </summary>
	[FromQuery(Name = "sort")]
	public NeoStatsSortBy Sort { get; init; } = NeoStatsSortBy.Date;

	/// <summary>
	/// Sort direction. Allowed values: Asc, Desc. Default: Asc
	/// </summary>
	[FromQuery(Name = "order")]
	public NeoSortOrder Order { get; init; } = NeoSortOrder.Asc;

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (From.HasValue && To.HasValue && From.Value.Date > To.Value.Date)
		{
			yield return new ValidationResult("'from' must be less than or equal to 'to'.", new[] { nameof(From), nameof(To) });
		}

		if (MinDiameter.HasValue && MinDiameter.Value < 0)
		{
			yield return new ValidationResult("'min_diameter' must be greater than or equal to 0.", new[] { nameof(MinDiameter) });
		}

		if (MaxDiameter.HasValue && MaxDiameter.Value < 0)
		{
			yield return new ValidationResult("'max_diameter' must be greater than or equal to 0.", new[] { nameof(MaxDiameter) });
		}

		if (MinDiameter.HasValue && MaxDiameter.HasValue && MinDiameter.Value > MaxDiameter.Value)
		{
			yield return new ValidationResult("'min_diameter' must be less than or equal to 'max_diameter'.", new[] { nameof(MinDiameter), nameof(MaxDiameter) });
		}
	}

	/// <summary>
	/// Build a stable cache key representing this set of query parameters.
	/// </summary>
	public string BuildCacheKey()
	{
		var from = From?.Date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) ?? "any";
		var to = To?.Date.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) ?? "any";
		var hazardous = Hazardous?.ToString() ?? "any";
		var minDiameter = MinDiameter?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "any";
		var maxDiameter = MaxDiameter?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "any";

		return $"from={from};to={to};hazardous={hazardous};min={minDiameter};max={maxDiameter};sort={Sort};order={Order}";
	}
}
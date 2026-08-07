using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace NeoWatcher.Dto;

public enum NeoStatsSortBy
{
	Date,
	ObjectCount,
	MaxDiameter,
	AvgVelocity
}

public enum NeoSortOrder
{
	Asc,
	Desc
}

public sealed record NeoStatsQuery : IValidatableObject
{
	[FromQuery(Name = "from")]
	public DateTime? From { get; init; }

	[FromQuery(Name = "to")]
	public DateTime? To { get; init; }

	[FromQuery(Name = "hazardous")]
	public bool? Hazardous { get; init; }

	[FromQuery(Name = "min_diameter")]
	public double? MinDiameter { get; init; }

	[FromQuery(Name = "max_diameter")]
	public double? MaxDiameter { get; init; }

	[FromQuery(Name = "sort")]
	public NeoStatsSortBy Sort { get; init; } = NeoStatsSortBy.Date;

	[FromQuery(Name = "order")]
	public NeoSortOrder Order { get; init; } = NeoSortOrder.Asc;

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (From.HasValue && To.HasValue && From.Value.Date > To.Value.Date)
		{
			yield return new ValidationResult("'from' must be less than or equal to 'to'.", [nameof(From), nameof(To)]);
		}

		if (MinDiameter.HasValue && MinDiameter.Value < 0)
		{
			yield return new ValidationResult("'min_diameter' must be greater than or equal to 0.", [nameof(MinDiameter)]);
		}

		if (MaxDiameter.HasValue && MaxDiameter.Value < 0)
		{
			yield return new ValidationResult("'max_diameter' must be greater than or equal to 0.", [nameof(MaxDiameter)]);
		}

		if (MinDiameter.HasValue && MaxDiameter.HasValue && MinDiameter.Value > MaxDiameter.Value)
		{
			yield return new ValidationResult("'min_diameter' must be less than or equal to 'max_diameter'.", [nameof(MinDiameter), nameof(MaxDiameter)]);
		}
	}

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
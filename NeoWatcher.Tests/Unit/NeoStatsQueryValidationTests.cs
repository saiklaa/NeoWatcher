using System.ComponentModel.DataAnnotations;
using System.Linq;
using NeoWatcher.Dto;
using Xunit;

namespace NeoWatcher.Tests.Unit;

public class NeoStatsQueryValidationTests
{
    [Fact]
    public void Validate_FromGreaterThanTo_ReturnsValidationError()
    {
        var q = new NeoStatsQuery
        {
            From = System.DateTime.Parse("2026-08-10"),
            To = System.DateTime.Parse("2026-08-01")
        };

        var ctx = new ValidationContext(q);
        var results = q.Validate(ctx).ToArray();

        Assert.Contains(results, r => r.ErrorMessage?.Contains("'from' must be less than or equal to 'to'.") == true);
    }
}

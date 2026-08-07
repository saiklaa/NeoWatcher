using System;
using NeoWatcher.Dto;
using Xunit;

namespace NeoWatcher.Tests.Unit;

public class NeoStatsQueryTests
{
    [Fact]
    public void BuildCacheKey_SameValues_ProduceSameKey()
    {
        var a = new NeoStatsQuery
        {
            From = DateTime.Parse("2026-08-01"),
            To = DateTime.Parse("2026-08-07"),
            Hazardous = true,
            MinDiameter = 10,
            MaxDiameter = 100,
            Sort = NeoStatsSortBy.MaxDiameter,
            Order = NeoSortOrder.Desc
        };

        var b = new NeoStatsQuery
        {
            From = DateTime.Parse("2026-08-01"),
            To = DateTime.Parse("2026-08-07"),
            Hazardous = true,
            MinDiameter = 10,
            MaxDiameter = 100,
            Sort = NeoStatsSortBy.MaxDiameter,
            Order = NeoSortOrder.Desc
        };

        Assert.Equal(a.BuildCacheKey(), b.BuildCacheKey());
    }
}

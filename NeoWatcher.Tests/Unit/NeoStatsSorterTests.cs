using System;
using System.Collections.Generic;
using FluentAssertions;
using NeoWatcher.Models;
using NeoWatcher.Services;
using Xunit;

namespace NeoWatcher.Tests.Unit;

public class NeoStatsSorterTests
{
    private List<NeoStatViewModel> Sample()
    {
        return new List<NeoStatViewModel>
        {
            new() { Date = DateTime.Parse("2026-08-02"), ObjectCount = 2, MaxDiameter = 100, AvgVelocity = 1000, HasHazardousObjects = false },
            new() { Date = DateTime.Parse("2026-08-01"), ObjectCount = 5, MaxDiameter = 50, AvgVelocity = 2000, HasHazardousObjects = true },
            new() { Date = DateTime.Parse("2026-08-03"), ObjectCount = 1, MaxDiameter = 200, AvgVelocity = 500, HasHazardousObjects = false },
        };
    }

    [Theory]
    [InlineData("date","asc","2026-08-01")]
    [InlineData("date","desc","2026-08-03")]
    [InlineData("count","asc","2026-08-03")]
    [InlineData("count","desc","2026-08-01")]
    [InlineData("mass","asc","2026-08-01")]
    [InlineData("mass","desc","2026-08-03")]
    public void ApplySort_OrdersAsExpected(string sortBy, string sortDir, string expectedFirstDate)
    {
        var items = Sample();
        var sorted = NeoStatsSorter.ApplySort(items, sortBy, sortDir);
        sorted.Should().NotBeEmpty();
        sorted[0].Date.ToString("yyyy-MM-dd").Should().Be(expectedFirstDate);
    }
}

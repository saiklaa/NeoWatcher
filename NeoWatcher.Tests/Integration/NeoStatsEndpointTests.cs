using System.Linq;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace NeoWatcher.Tests.Integration;

public record NeoStatResponseDto(
    System.DateTime Date,
    int ObjectCount,
    double MaxDiameter,
    double AvgVelocity,
    bool HasHazardousObjects);

public class NeoStatsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public NeoStatsEndpointTests(WebApplicationFactory<Program> factory) => _factory = factory;

    [Fact]
    public async Task GetStats_Default_ReturnsGroupedOrderedByDate()
    {
        using var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/neo/stats");
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync();
        var docs = JsonSerializer.Deserialize<NeoStatResponseDto[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        docs.Should().NotBeNull();
        docs!.Length.Should().BeGreaterThan(0);

        var dates = docs.Select(d => d.Date).ToArray();
        dates.Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task GetStats_SortByObjectCountDesc_ReturnsSortedByCountDescending()
    {
        using var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/neo/stats?sort=ObjectCount&order=desc");
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync();
        var docs = JsonSerializer.Deserialize<NeoStatResponseDto[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        docs.Should().NotBeNull();
        docs!.Length.Should().BeGreaterThan(1);

        var counts = docs.Select(d => d.ObjectCount).ToArray();
        counts.Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task GetStats_SortByMaxDiameterAsc_ReturnsSortedByDiameterAscending()
    {
        using var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/neo/stats?sort=MaxDiameter&order=asc");
        res.EnsureSuccessStatusCode();

        var json = await res.Content.ReadAsStringAsync();
        var docs = JsonSerializer.Deserialize<NeoStatResponseDto[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        docs.Should().NotBeNull();
        docs!.Length.Should().BeGreaterThan(1);

        var diameters = docs.Select(d => d.MaxDiameter).ToArray();
        diameters.Should().BeInAscendingOrder();
    }

using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
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
}

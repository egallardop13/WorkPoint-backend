using System.Net;
using FluentAssertions;

namespace DotnetAPI.Tests.IntegrationTests;

public class HealthCheckTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PingEndpoint_ReturnsPong()
    {
        var response = await _client.GetAsync("/ping");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("pong");
    }

    [Fact]
    public async Task PingEndpoint_DoesNotRequireAuth()
    {
        var response = await _client.GetAsync("/ping");

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
}

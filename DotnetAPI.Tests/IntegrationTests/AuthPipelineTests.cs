using System.Net;
using System.Text;
using FluentAssertions;

namespace DotnetAPI.Tests.IntegrationTests;

public class AuthPipelineTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthPipelineTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/UserComplete/GetUsers/0/true");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithEmptyBody_ReturnsNon200()
    {
        var content = new StringContent("{}", Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/Auth/Login", content);

        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }
}

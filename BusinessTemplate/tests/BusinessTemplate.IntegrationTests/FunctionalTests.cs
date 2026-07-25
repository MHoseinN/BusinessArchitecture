using System.Net;
using System.Net.Http.Json;
using BusinessTemplate.Modules.Identity.Features.RegisterUser;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BusinessTemplate.IntegrationTests;

public sealed class FunctionalTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public FunctionalTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_should_be_available()
    {
        var response = await _client.GetAsync("/health/live");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Register_validation_failure_should_return_400()
    {
        var response = await _client.PostAsJsonAsync("/api/identity/users", new Request("", "", null, ""));
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

using FluentAssertions;
using Testcontainers.PostgreSql;

namespace BusinessTemplate.IntegrationTests;

public sealed class TestcontainersSmokeTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("business_template_test")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    [Fact]
    public void Placeholder() => true.Should().BeTrue();

    public Task InitializeAsync() => _container.StartAsync();

    public async Task DisposeAsync() => await _container.DisposeAsync();
}

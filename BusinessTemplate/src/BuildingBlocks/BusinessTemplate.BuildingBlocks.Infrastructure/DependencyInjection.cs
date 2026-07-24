using BusinessTemplate.BuildingBlocks.Application.Abstractions;
using BusinessTemplate.BuildingBlocks.Infrastructure.Abstractions;
using BusinessTemplate.BuildingBlocks.Infrastructure.Identity;
using BusinessTemplate.BuildingBlocks.Infrastructure.Outbox;
using BusinessTemplate.BuildingBlocks.Infrastructure.Tenancy;
using BusinessTemplate.BuildingBlocks.Infrastructure.Time;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessTemplate.BuildingBlocks.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBuildingBlocks(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IClock, SystemClock>();
        services.AddScoped<ICurrentUser, CurrentUserAccessor>();
        services.AddScoped<ITenantResolver, HeaderTenantResolver>();
        services.AddScoped<ITenantContext, TenantContext>();
        services.AddSingleton<InMemoryOutboxStore>();
        services.AddSingleton<IOutboxStore>(sp => sp.GetRequiredService<InMemoryOutboxStore>());
        services.AddSingleton<IIntegrationEventPublisher, InProcessIntegrationEventPublisher>();
        services.AddHostedService<OutboxProcessorWorker>();
        return services;
    }
}

using BusinessTemplate.BuildingBlocks.Contracts;

namespace BusinessTemplate.BuildingBlocks.Infrastructure.Abstractions;

public interface IIntegrationEventPublisher
{
    Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}

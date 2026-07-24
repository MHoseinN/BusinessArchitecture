using BusinessTemplate.BuildingBlocks.Contracts;
using BusinessTemplate.BuildingBlocks.Infrastructure.Abstractions;
using System.Text.Json;

namespace BusinessTemplate.BuildingBlocks.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTimeOffset OccurredOnUtc { get; set; }
    public DateTimeOffset? ProcessedOnUtc { get; set; }
    public int RetryCount { get; set; }
    public string? Error { get; set; }
}

public sealed class InboxMessage
{
    public Guid Id { get; set; }
    public string MessageId { get; set; } = string.Empty;
    public DateTimeOffset ProcessedOnUtc { get; set; }
}

public sealed class IdempotencyKey
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public DateTimeOffset CreatedOnUtc { get; set; }
}

public interface IOutboxStore
{
    Task AddAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}

public sealed class InMemoryOutboxStore : IOutboxStore
{
    public List<OutboxMessage> Messages { get; } = [];

    public Task AddAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        Messages.Add(new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = integrationEvent.EventName,
            Payload = JsonSerializer.Serialize(integrationEvent),
            OccurredOnUtc = integrationEvent.OccurredOnUtc
        });

        return Task.CompletedTask;
    }
}

public sealed class InProcessIntegrationEventPublisher : IIntegrationEventPublisher
{
    public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken) => Task.CompletedTask;
}

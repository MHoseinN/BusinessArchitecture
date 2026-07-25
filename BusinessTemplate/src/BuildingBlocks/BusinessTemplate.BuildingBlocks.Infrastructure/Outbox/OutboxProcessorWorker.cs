using BusinessTemplate.BuildingBlocks.Infrastructure.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BusinessTemplate.BuildingBlocks.Infrastructure.Outbox;

public sealed class OutboxProcessorWorker(
    InMemoryOutboxStore outboxStore,
    IIntegrationEventPublisher publisher,
    ILogger<OutboxProcessorWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var message in outboxStore.Messages.Where(m => m.ProcessedOnUtc is null).ToList())
            {
                try
                {
                    await publisher.PublishAsync(new ProcessedOutboxIntegrationEvent(message.Type), stoppingToken);
                    message.ProcessedOnUtc = DateTimeOffset.UtcNow;
                }
                catch (Exception ex)
                {
                    message.RetryCount++;
                    message.Error = ex.Message;
                    logger.LogError(ex, "Outbox processing failed for {MessageId}", message.Id);
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private sealed record ProcessedOutboxIntegrationEvent(string EventName) : Contracts.IntegrationEvent(EventName);
}

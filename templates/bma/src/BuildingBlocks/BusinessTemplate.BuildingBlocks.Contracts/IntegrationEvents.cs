namespace BusinessTemplate.BuildingBlocks.Contracts;

public interface IIntegrationEvent
{
    string EventName { get; }
    DateTimeOffset OccurredOnUtc { get; }
}

public abstract record IntegrationEvent(string EventName) : IIntegrationEvent
{
    public DateTimeOffset OccurredOnUtc { get; init; } = DateTimeOffset.UtcNow;
}

public sealed record ContractVersion(int Major, int Minor)
{
    public override string ToString() => $"{Major}.{Minor}";
}

public readonly record struct CorrelationId(string Value);
public readonly record struct CausationId(string Value);

public sealed record EventEnvelope<TEvent>(
    TEvent Data,
    ContractVersion Version,
    CorrelationId CorrelationId,
    CausationId CausationId)
    where TEvent : IIntegrationEvent;

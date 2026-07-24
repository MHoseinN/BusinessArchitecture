namespace BusinessTemplate.BuildingBlocks.Domain.Primitives;

public abstract record DomainEvent : IDomainEvent
{
    public DateTimeOffset OccurredOnUtc { get; init; } = DateTimeOffset.UtcNow;
}

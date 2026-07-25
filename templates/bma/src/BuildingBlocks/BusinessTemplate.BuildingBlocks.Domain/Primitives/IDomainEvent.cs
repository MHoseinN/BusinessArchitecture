namespace BusinessTemplate.BuildingBlocks.Domain.Primitives;

public interface IDomainEvent
{
    DateTimeOffset OccurredOnUtc { get; }
}

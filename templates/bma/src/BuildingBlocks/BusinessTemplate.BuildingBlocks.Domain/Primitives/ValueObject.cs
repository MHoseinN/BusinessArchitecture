namespace BusinessTemplate.BuildingBlocks.Domain.Primitives;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(ValueObject? other) =>
        other is not null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override bool Equals(object? obj) => obj is ValueObject other && Equals(other);
    public override int GetHashCode() => GetEqualityComponents().Aggregate(17, (current, obj) => current * 31 + (obj?.GetHashCode() ?? 0));
}

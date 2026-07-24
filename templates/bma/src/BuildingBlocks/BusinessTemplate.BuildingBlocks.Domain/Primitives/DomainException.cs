namespace BusinessTemplate.BuildingBlocks.Domain.Primitives;

public sealed class DomainException : Exception
{
    public DomainException(Error error) : base(error.Message) => Error = error;

    public Error Error { get; }
}

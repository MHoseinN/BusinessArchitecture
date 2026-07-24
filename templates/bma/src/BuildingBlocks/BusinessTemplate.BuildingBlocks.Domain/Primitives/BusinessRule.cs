namespace BusinessTemplate.BuildingBlocks.Domain.Primitives;

public abstract class BusinessRule
{
    public abstract string Code { get; }
    public abstract string Message { get; }
    public abstract bool IsBroken();

    public Error ToError() => new(Code, Message, ErrorType.Validation);
}

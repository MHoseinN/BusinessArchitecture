using System.Text.RegularExpressions;
using BusinessTemplate.BuildingBlocks.Domain.Primitives;

namespace BusinessTemplate.Modules.Identity.Domain;

public readonly record struct UserId(Guid Value)
{
    public static implicit operator Guid(UserId id) => id.Value;
    public static implicit operator UserId(Guid value) => new(value);
}
public readonly record struct RoleId(Guid Value)
{
    public static implicit operator Guid(RoleId id) => id.Value;
    public static implicit operator RoleId(Guid value) => new(value);
}

public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex Regex = new(@"^\+?[1-9]\d{7,14}$", RegexOptions.Compiled);
    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static Result<PhoneNumber> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value))
        {
            return Result<PhoneNumber>.Failure(new Error("identity.phone.invalid", "Phone number is invalid.", ErrorType.Validation));
        }

        return Result<PhoneNumber>.Success(new PhoneNumber(value.Trim()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}

public sealed class EmailAddress : ValueObject
{
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    public static Result<EmailAddress?> CreateOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<EmailAddress?>.Success(null);
        }

        if (!value.Contains('@'))
        {
            return Result<EmailAddress?>.Failure(new Error("identity.email.invalid", "Email is invalid.", ErrorType.Validation));
        }

        return Result<EmailAddress?>.Success(new EmailAddress(value.Trim()));
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}

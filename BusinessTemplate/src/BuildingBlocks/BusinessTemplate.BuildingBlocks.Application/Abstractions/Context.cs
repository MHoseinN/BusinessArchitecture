namespace BusinessTemplate.BuildingBlocks.Application.Abstractions;

public interface ICurrentUser
{
    string? UserId { get; }
    string? Email { get; }
    IReadOnlyCollection<string> Permissions { get; }
}

public interface IClock
{
    DateTimeOffset UtcNow { get; }
}

public interface ITenantContext
{
    string? TenantId { get; }
}

public interface ITenantResolver
{
    string? ResolveTenantId();
}

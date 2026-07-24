namespace BusinessTemplate.Modules.Identity.Infrastructure.Persistence;

public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTimeOffset ExpiresOnUtc { get; set; }
    public bool IsRevoked { get; set; }
}

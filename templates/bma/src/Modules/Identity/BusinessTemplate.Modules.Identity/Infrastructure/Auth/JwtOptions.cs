using System.ComponentModel.DataAnnotations;

namespace BusinessTemplate.Modules.Identity.Infrastructure.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = "BusinessTemplate";

    [Required]
    public string Audience { get; init; } = "BusinessTemplate.Client";

    [Required]
    [MinLength(32)]
    public string Secret { get; init; } = "DevelopmentOnlySecretKeyWithEnoughLength1234";

    public int AccessTokenMinutes { get; init; } = 30;
    public int RefreshTokenDays { get; init; } = 7;
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessTemplate.Modules.Identity.Features.Auth;
using BusinessTemplate.Modules.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BusinessTemplate.Modules.Identity.Infrastructure.Auth;

public interface IJwtTokenService
{
    Task<LoginResponse?> LoginAsync(string phoneNumber, string password, CancellationToken cancellationToken);
    Task<LoginResponse?> RefreshAsync(string refreshToken, CancellationToken cancellationToken);
}

internal sealed class JwtTokenService(IdentityDbContext dbContext, IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _options = options.Value;

    public async Task<LoginResponse?> LoginAsync(string phoneNumber, string password, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.PhoneNumber.Value == phoneNumber, cancellationToken);
        if (user is null || user.Status != Domain.UserStatus.Active || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return null;
        }

        return await IssueTokensAsync(user.Id.Value, cancellationToken);
    }

    public async Task<LoginResponse?> RefreshAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refreshToken && !x.IsRevoked, cancellationToken);

        if (token is null || token.ExpiresOnUtc <= DateTimeOffset.UtcNow)
        {
            return null;
        }

        token.IsRevoked = true;
        return await IssueTokensAsync(token.UserId, cancellationToken);
    }

    private async Task<LoginResponse> IssueTokensAsync(Guid userId, CancellationToken cancellationToken)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new("permission", IdentityPermissions.UsersRead),
            new("permission", IdentityPermissions.UsersWrite)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var access = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes),
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(access);
        var refresh = Guid.NewGuid().ToString("N");

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = refresh,
            ExpiresOnUtc = DateTimeOffset.UtcNow.AddDays(_options.RefreshTokenDays)
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return new LoginResponse(accessToken, refresh);
    }
}

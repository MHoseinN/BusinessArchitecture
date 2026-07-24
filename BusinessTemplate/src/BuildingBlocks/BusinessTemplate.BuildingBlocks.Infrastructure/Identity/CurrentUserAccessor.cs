using System.Security.Claims;
using BusinessTemplate.BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace BusinessTemplate.BuildingBlocks.Infrastructure.Identity;

public sealed class CurrentUserAccessor(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Email => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
    public IReadOnlyCollection<string> Permissions =>
        httpContextAccessor.HttpContext?.User.FindAll("permission").Select(x => x.Value).ToArray() ?? [];
}

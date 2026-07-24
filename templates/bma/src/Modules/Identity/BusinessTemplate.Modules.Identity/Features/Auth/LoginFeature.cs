using BusinessTemplate.Modules.Identity.Infrastructure.Auth;
using FastEndpoints;

namespace BusinessTemplate.Modules.Identity.Features.Auth;

public sealed record LoginRequest(string PhoneNumber, string Password);
public sealed record LoginResponse(string AccessToken, string RefreshToken);
public sealed record RefreshRequest(string RefreshToken);

public sealed class LoginEndpoint(IJwtTokenService tokenService) : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/api/identity/auth/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await tokenService.LoginAsync(req.PhoneNumber, req.Password, ct);
        if (result is null) { await Send.UnauthorizedAsync(ct); return; }
        await Send.OkAsync(result, ct);
    }
}

public sealed class RefreshEndpoint(IJwtTokenService tokenService) : Endpoint<RefreshRequest, LoginResponse>
{
    public override void Configure()
    {
        Post("/api/identity/auth/refresh");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RefreshRequest req, CancellationToken ct)
    {
        var result = await tokenService.RefreshAsync(req.RefreshToken, ct);
        if (result is null) { await Send.UnauthorizedAsync(ct); return; }
        await Send.OkAsync(result, ct);
    }
}

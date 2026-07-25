using BusinessTemplate.Modules.Identity.Infrastructure.Persistence;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BusinessTemplate.Modules.Identity.Features.AssignRoleToUser;

public sealed record Request(Guid Id, Guid RoleId);

public sealed class Endpoint(IdentityDbContext dbContext) : Endpoint<Request>
{
    public override void Configure()
    {
        Post("/api/identity/users/{id:guid}/roles/{roleId:guid}");
        Permissions(IdentityPermissions.UsersWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == new Domain.UserId(req.Id), ct);
        if (user is null) { await Send.NotFoundAsync(ct); return; }
        var result = user.AssignRole(new Domain.RoleId(req.RoleId));
        if (result.IsFailure) { await Send.StringAsync($"{result.Error.Code}:{result.Error.Message}", statusCode: 409, cancellation: ct); return; }
        await dbContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}

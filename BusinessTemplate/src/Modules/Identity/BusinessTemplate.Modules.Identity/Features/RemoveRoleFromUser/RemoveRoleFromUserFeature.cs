using BusinessTemplate.Modules.Identity.Infrastructure.Persistence;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BusinessTemplate.Modules.Identity.Features.RemoveRoleFromUser;

public sealed record Request(Guid Id, Guid RoleId);

public sealed class Endpoint(IdentityDbContext dbContext) : Endpoint<Request>
{
    public override void Configure()
    {
        Delete("/api/identity/users/{id:guid}/roles/{roleId:guid}");
        Permissions(IdentityPermissions.UsersWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == new Domain.UserId(req.Id), ct);
        if (user is null) { await Send.NotFoundAsync(ct); return; }
        user.RemoveRole(new Domain.RoleId(req.RoleId));
        await dbContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}

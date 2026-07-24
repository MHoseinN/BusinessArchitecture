using BusinessTemplate.Modules.Identity.Infrastructure.Persistence;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BusinessTemplate.Modules.Identity.Features.UpdateUser;

public sealed record Request(Guid Id, string Name, string? Email);

public sealed class Endpoint(IdentityDbContext dbContext) : Endpoint<Request>
{
    public override void Configure()
    {
        Put("/api/identity/users/{id:guid}");
        Permissions(IdentityPermissions.UsersWrite);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == new Domain.UserId(req.Id), ct);
        if (user is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var result = user.Update(req.Name, req.Email);
        if (result.IsFailure)
        {
            await Send.StringAsync($"{result.Error.Code}:{result.Error.Message}", statusCode: 400, cancellation: ct);
            return;
        }

        await dbContext.SaveChangesAsync(ct);
        await Send.NoContentAsync(ct);
    }
}

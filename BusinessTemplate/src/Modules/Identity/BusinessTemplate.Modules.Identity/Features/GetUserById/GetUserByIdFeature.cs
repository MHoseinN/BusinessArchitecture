using BusinessTemplate.BuildingBlocks.Domain.Primitives;
using BusinessTemplate.Modules.Identity.Infrastructure.Persistence;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BusinessTemplate.Modules.Identity.Features.GetUserById;

public sealed record Request(Guid Id);
public sealed record Response(Guid Id, string Name, string PhoneNumber, string? Email, string Status);
public sealed record Query(Guid Id);

public sealed class Handler(IdentityDbContext dbContext)
{
    public async Task<Result<Response>> HandleAsync(Query query, CancellationToken cancellationToken)
    {
        var item = await dbContext.Users.AsNoTracking()
            .Where(x => x.Id == new Domain.UserId(query.Id))
            .Select(x => new Response(x.Id.Value, x.Name, x.PhoneNumber.Value, x.EmailAddress == null ? null : x.EmailAddress.Value, x.Status.ToString()))
            .FirstOrDefaultAsync(cancellationToken);

        return item is null
            ? Result<Response>.Failure(new Error("identity.user.not_found", "User not found.", ErrorType.NotFound))
            : Result<Response>.Success(item);
    }
}

public sealed class Endpoint(Handler handler) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Get("/api/identity/users/{id:guid}");
        Permissions(IdentityPermissions.UsersRead);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await handler.HandleAsync(new Query(req.Id), ct);
        if (result.IsFailure)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        await Send.OkAsync(result.Value!, ct);
    }
}

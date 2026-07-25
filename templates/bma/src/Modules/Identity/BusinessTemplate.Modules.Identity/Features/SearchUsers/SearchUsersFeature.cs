using BusinessTemplate.BuildingBlocks.Application.Models;
using BusinessTemplate.Modules.Identity.Infrastructure.Persistence;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace BusinessTemplate.Modules.Identity.Features.SearchUsers;

public sealed record Request(int Page = 1, int PageSize = 20, string? Search = null, string? Status = null);
public sealed record Item(Guid Id, string Name, string PhoneNumber, string? Email, string Status);

public sealed class Endpoint(IdentityDbContext dbContext) : Endpoint<Request, PagedResult<Item>>
{
    public override void Configure()
    {
        Get("/api/identity/users");
        Permissions(IdentityPermissions.UsersRead);
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var query = dbContext.Users.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            query = query.Where(x => x.Name.Contains(req.Search) || x.PhoneNumber.Value.Contains(req.Search));
        }

        if (!string.IsNullOrWhiteSpace(req.Status) && Enum.TryParse<Domain.UserStatus>(req.Status, true, out var status))
        {
            query = query.Where(x => x.Status == status);
        }

        var page = req.Page < 1 ? 1 : req.Page;
        var pageSize = Math.Clamp(req.PageSize, 1, 100);
        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.CreatedOnUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new Item(x.Id.Value, x.Name, x.PhoneNumber.Value, x.EmailAddress == null ? null : x.EmailAddress.Value, x.Status.ToString()))
            .ToListAsync(ct);

        await Send.OkAsync(new PagedResult<Item>(page, pageSize, total, items), ct);
    }
}

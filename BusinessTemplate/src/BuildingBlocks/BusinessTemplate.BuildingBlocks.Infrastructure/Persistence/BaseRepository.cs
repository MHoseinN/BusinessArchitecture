using Microsoft.EntityFrameworkCore;

namespace BusinessTemplate.BuildingBlocks.Infrastructure.Persistence;

internal abstract class BaseRepository<TDbContext>(TDbContext dbContext)
    where TDbContext : DbContext
{
    protected TDbContext DbContext { get; } = dbContext;
}

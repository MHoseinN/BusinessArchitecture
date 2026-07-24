using BusinessTemplate.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace BusinessTemplate.Modules.Identity.Infrastructure.Persistence;

internal sealed class IdentityRepository(IdentityDbContext dbContext) : IUserRepository
{
    public Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken) =>
        dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> PhoneNumberExistsAsync(string phoneNumber, CancellationToken cancellationToken) =>
        dbContext.Users.AnyAsync(x => x.PhoneNumber.Value == phoneNumber, cancellationToken);

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        dbContext.Users.Add(user);
        return Task.CompletedTask;
    }
}

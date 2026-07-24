namespace BusinessTemplate.Modules.Identity.Domain;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken);
    Task<bool> PhoneNumberExistsAsync(string phoneNumber, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
}

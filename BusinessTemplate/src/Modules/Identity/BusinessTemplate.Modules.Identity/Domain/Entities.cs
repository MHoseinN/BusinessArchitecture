using BusinessTemplate.BuildingBlocks.Domain.Primitives;

namespace BusinessTemplate.Modules.Identity.Domain;

public sealed partial class User : AggregateRoot<UserId>
{
    private readonly HashSet<RoleId> _roles = [];

    private User(UserId id, string name, PhoneNumber phoneNumber, EmailAddress? emailAddress) : base(id)
    {
        Name = name;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
        Status = UserStatus.Active;
        CreatedOnUtc = DateTimeOffset.UtcNow;
        RaiseDomainEvent(new UserRegisteredDomainEvent(id));
    }

    private User() : base(default) { Name = string.Empty; PhoneNumber = null!; CreatedOnUtc = DateTimeOffset.UtcNow; }

    public string Name { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public EmailAddress? EmailAddress { get; private set; }
    public UserStatus Status { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; private set; }
    public IReadOnlyCollection<RoleId> Roles => _roles;

    public static Result<User> Register(UserId id, string name, PhoneNumber phoneNumber, EmailAddress? emailAddress)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<User>.Failure(new Error("identity.user.name_required", "Name is required.", ErrorType.Validation));
        }

        return Result<User>.Success(new User(id, name.Trim(), phoneNumber, emailAddress));
    }

    public Result Update(string name, string? email)
    {
        if (Status == UserStatus.Deleted)
        {
            return Result.Failure(new Error("identity.user.deleted", "Deleted user cannot be updated.", ErrorType.Conflict));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(new Error("identity.user.name_required", "Name is required.", ErrorType.Validation));
        }

        var emailResult = EmailAddress.CreateOptional(email);
        if (emailResult.IsFailure)
        {
            return Result.Failure(emailResult.Error);
        }

        Name = name.Trim();
        EmailAddress = emailResult.Value;
        return Result.Success();
    }

    public Result Suspend()
    {
        if (Status == UserStatus.Deleted)
        {
            return Result.Failure(new Error("identity.user.deleted", "Deleted user cannot be suspended.", ErrorType.Conflict));
        }

        Status = UserStatus.Suspended;
        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == UserStatus.Deleted)
        {
            return Result.Failure(new Error("identity.user.deleted", "Deleted user cannot be activated.", ErrorType.Conflict));
        }

        Status = UserStatus.Active;
        return Result.Success();
    }

    public Result Delete()
    {
        Status = UserStatus.Deleted;
        return Result.Success();
    }

    public Result AssignRole(RoleId roleId)
    {
        if (!_roles.Add(roleId))
        {
            return Result.Failure(new Error("identity.user.role_exists", "Role already assigned.", ErrorType.Conflict));
        }

        return Result.Success();
    }

    public Result RemoveRole(RoleId roleId)
    {
        _roles.Remove(roleId);
        return Result.Success();
    }
}

public sealed class Role : Entity<RoleId>
{
    private Role() : base(default) { Name = string.Empty; }
    public Role(RoleId id, string name) : base(id) => Name = name;
    public string Name { get; private set; }
}

public sealed class Permission : Entity<Guid>
{
    private Permission() : base(Guid.Empty) { Name = string.Empty; }
    public Permission(Guid id, string name) : base(id) => Name = name;
    public string Name { get; private set; }
}

public sealed partial class UserRole
{
    public UserId UserId { get; set; }
    public RoleId RoleId { get; set; }
}

public sealed class RolePermission
{
    public RoleId RoleId { get; set; }
    public Guid PermissionId { get; set; }
}

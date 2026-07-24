using BusinessTemplate.BuildingBlocks.Domain.Primitives;
using BusinessTemplate.BuildingBlocks.Infrastructure.Outbox;
using BusinessTemplate.Modules.Identity.Contracts;
using BusinessTemplate.Modules.Identity.Domain;
using BusinessTemplate.Modules.Identity.Infrastructure.Persistence;
using FastEndpoints;
using FluentValidation;

namespace BusinessTemplate.Modules.Identity.Features.RegisterUser;

public sealed record Request(string Name, string PhoneNumber, string? Email, string Password);
public sealed record Response(Guid UserId);
public sealed record Command(string Name, string PhoneNumber, string? Email, string Password);

public sealed class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public sealed class Handler(IUserRepository repository, IdentityDbContext dbContext, IOutboxStore outboxStore)
{
    public async Task<Result<Response>> HandleAsync(Command command, CancellationToken cancellationToken)
    {
        var phoneResult = PhoneNumber.Create(command.PhoneNumber);
        if (phoneResult.IsFailure) return Result<Response>.Failure(phoneResult.Error);

        var emailResult = EmailAddress.CreateOptional(command.Email);
        if (emailResult.IsFailure) return Result<Response>.Failure(emailResult.Error);

        if (await repository.PhoneNumberExistsAsync(phoneResult.Value!.Value, cancellationToken))
        {
            return Result<Response>.Failure(new Error("identity.user.phone_exists", "Phone number already exists.", ErrorType.Conflict));
        }

        var userResult = User.Register(new UserId(Guid.NewGuid()), command.Name, phoneResult.Value!, emailResult.Value);
        if (userResult.IsFailure) return Result<Response>.Failure(userResult.Error);

        userResult.Value!.SetPasswordHash(BCrypt.Net.BCrypt.HashPassword(command.Password));
        await repository.AddAsync(userResult.Value!, cancellationToken);
        await outboxStore.AddAsync(new UserRegisteredIntegrationEvent(userResult.Value!.Id.Value, userResult.Value.Name, userResult.Value.PhoneNumber.Value), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Response>.Success(new Response(userResult.Value.Id.Value));
    }
}

public sealed class Endpoint(Handler handler) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/api/identity/users");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await handler.HandleAsync(new Command(req.Name, req.PhoneNumber, req.Email, req.Password), ct);
        if (result.IsFailure)
        {
            await Send.StringAsync($"{result.Error.Code}:{result.Error.Message}", statusCode: result.Error.Type == ErrorType.Conflict ? 409 : 400, cancellation: ct);
            return;
        }

        await Send.OkAsync(result.Value!, ct);
    }
}

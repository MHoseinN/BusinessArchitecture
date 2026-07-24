using BusinessTemplate.BuildingBlocks.Contracts;

namespace BusinessTemplate.Modules.Identity.Contracts;

public sealed record UserRegisteredIntegrationEvent(Guid UserId, string Name, string PhoneNumber)
    : IntegrationEvent("identity.user.registered");

using BusinessTemplate.BuildingBlocks.Domain.Primitives;

namespace BusinessTemplate.Modules.Identity.Domain;

public sealed record UserRegisteredDomainEvent(UserId UserId) : DomainEvent;

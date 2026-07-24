using BusinessTemplate.BuildingBlocks.Application.Abstractions;

namespace BusinessTemplate.BuildingBlocks.Infrastructure.Time;

public sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}

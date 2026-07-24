using BusinessTemplate.BuildingBlocks.Application.Abstractions;
using Microsoft.AspNetCore.Http;

namespace BusinessTemplate.BuildingBlocks.Infrastructure.Tenancy;

public sealed class HeaderTenantResolver(IHttpContextAccessor accessor) : ITenantResolver
{
    public string? ResolveTenantId() => accessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
}

public sealed class TenantContext(ITenantResolver resolver) : ITenantContext
{
    public string? TenantId => resolver.ResolveTenantId();
}

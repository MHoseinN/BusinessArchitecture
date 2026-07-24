using BusinessTemplate.BuildingBlocks.Infrastructure;
using BusinessTemplate.Modules.Identity;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddBuildingBlocks()
    .AddIdentityModule(builder.Configuration)
    .AddFastEndpoints()
    .SwaggerDocument();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    if (builder.Configuration.GetValue<bool>("AutoMigrateOnStartup"))
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BusinessTemplate.Modules.Identity.Infrastructure.Persistence.IdentityDbContext>();
        await db.Database.MigrateAsync();
    }

    app.UseSwaggerGen();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready");

app.Run();

public partial class Program;

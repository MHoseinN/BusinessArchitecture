namespace BusinessTemplate.Modules.SampleModule;

public static class DependencyInjection
{
    public static IServiceCollection AddSampleModule(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}

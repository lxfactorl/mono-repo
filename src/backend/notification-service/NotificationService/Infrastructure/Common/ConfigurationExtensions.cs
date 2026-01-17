using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace NotificationService.Infrastructure.Common;

public static class ConfigurationExtensions
{
    public static IServiceCollection AddValidatedOptions<TOptions>(this IServiceCollection services, IConfiguration configuration, string sectionName)
        where TOptions : class, new()
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<TOptions>()
            .Bind(configuration.GetSection(sectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<TOptions>>().Value);

        return services;
    }
}

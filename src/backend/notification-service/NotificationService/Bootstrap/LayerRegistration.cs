using Microsoft.Extensions.DependencyInjection;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Providers;

namespace NotificationService.Bootstrap;

public static class LayerRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<INotificationProvider, LoggingProvider>();

        // TelegramProvider will be registered in Phase 2 when ITelegramMessenger adapter is implemented
        // For now, tests register it explicitly with mocked dependencies

        return services;
    }
}

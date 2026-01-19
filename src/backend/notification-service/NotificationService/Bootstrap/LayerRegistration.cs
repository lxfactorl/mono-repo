using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using NotificationService.Api.Models.Settings;
using NotificationService.Application.Interfaces;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Providers;
using NotificationService.Infrastructure.Providers.Telegram;
using Telegram.Bot;

namespace NotificationService.Bootstrap;

public static class LayerRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        // Always register LoggingProvider as fallback
        services.AddScoped<INotificationProvider, LoggingProvider>();

        // Conditionally register TelegramProvider when BotToken is configured
        var telegramSection = configuration.GetSection("Telegram");
        var botToken = telegramSection.GetValue<string>("BotToken");

        if (!string.IsNullOrWhiteSpace(botToken))
        {
            Log.Information("Telegram provider enabled for NotificationService.");
            // Register TelegramBotClient with the configured token
            services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(botToken));

            // Register adapter and provider
            services.AddScoped<ITelegramMessenger, TelegramBotAdapter>();
            services.AddScoped<INotificationProvider, TelegramProvider>();
        }
        else
        {
            Log.Warning("Telegram provider skipped: No BotToken found in configuration.");
        }

        return services;
    }
}

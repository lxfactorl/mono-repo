using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Api.Endpoints;
using NotificationService.Api.Models.Settings;
using NotificationService.Infrastructure.Common;
using Scalar.AspNetCore;
using Serilog;

namespace NotificationService.Bootstrap;

public static class ServiceBootstrap
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // 1. Logging
        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(context.Configuration);
        });

        // 2. OpenAPI & Scalar
        builder.Services.AddOpenApi();

        // 3. Layered DI & Settings
        builder.Services
            .AddValidatedOptions<AppNotificationSettings>(builder.Configuration, "NotificationSettings")
            .AddApplication()
            .AddInfrastructure(builder.Configuration);


        // Telegram settings registration (conditional)
        // Only enabled if BotToken is present to allow graceful fallback
        var telegramSection = builder.Configuration.GetSection("Telegram");
        if (!string.IsNullOrWhiteSpace(telegramSection.GetValue<string>("BotToken")))
        {
            builder.Services.AddValidatedOptions<TelegramSettings>(builder.Configuration, "Telegram");
        }

        return builder;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // 1. Documentation UI
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        // 2. Endpoints
        app.MapNotificationEndpoints();

        return app;
    }
}

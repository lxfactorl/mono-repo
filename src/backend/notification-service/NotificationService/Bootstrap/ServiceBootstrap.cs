using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NotificationService.Api.Endpoints;
using NotificationService.Api.Models.Settings;
using NotificationService.Infrastructure.Common;
using NotificationService.Infrastructure.Middleware;
using Microsoft.AspNetCore.HttpOverrides;
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

        // 1.1 Proxy Support (Critical for Railway/Container environments)
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            // Railway uses dynamic internal IPs, so we must clear these checks to trust the incoming headers
            options.KnownIPNetworks.Clear();
            options.KnownProxies.Clear();
        });

        // 2. Exception Handling
        builder.Services.AddSingleton(new GlobalExceptionHandlerOptions
        {
            // Example: Map multiple custom exception types using a switch expression
            // CustomExceptionMapper = exception => exception switch
            // {
            //     ValidationException => (400, "Validation failed. Please check your input."),
            //     NotFoundException => (404, "The requested resource was not found."),
            //     BusinessRuleException => (422, "Business rule violation occurred."),
            //     _ => null  // Return null to use default mapping for unhandled types
            // }
        });
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails(); // Required for UseExceptionHandler() middleware

        // 3. OpenAPI & Scalar
        builder.Services.AddOpenApi();

        // 4. Layered DI & Settings
        builder.Services
            .AddValidatedOptions<AppNotificationSettings>(builder.Configuration, "NotificationSettings")
            .AddApplication()
            .AddInfrastructure(builder.Configuration);


        // Telegram settings registration (mandatory)
        // This will throw OptionsValidationException on startup if config is missing/invalid
        builder.Services.AddValidatedOptions<TelegramSettings>(builder.Configuration, "Telegram");

        return builder;
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        // 0. Forwarded Headers (Must be first to correctly identify scheme/IP)
        app.UseForwardedHeaders();

        // 1. Exception Handling (must be early in pipeline)
        app.UseExceptionHandler();

        // 2. Documentation UI
        app.MapOpenApi();
        app.MapScalarApiReference();

        // 3. Health Check (Railway deployment verification)
        app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
            .WithName("HealthCheck")
            .WithTags("Infrastructure")
            .WithDescription("Health check endpoint for deployment verification");

        // 4. Endpoints
        app.MapNotificationEndpoints();

        return app;
    }
}

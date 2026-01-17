using Microsoft.Extensions.Logging;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Providers;

internal class LoggingProvider : INotificationProvider
{
    private readonly ILogger<LoggingProvider> _logger;

    public LoggingProvider(ILogger<LoggingProvider> logger)
    {
        _logger = logger;
    }

    public string ProviderName => "LoggingProvider";

    public Task SendAsync(NotificationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation("[{ProviderName}] Sending notification to {Recipient}: {Message}", ProviderName, request.Recipient, request.Message);
        return Task.CompletedTask;
    }
}

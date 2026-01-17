using Microsoft.Extensions.Logging;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Application.Services;

public class NotificationDispatcher : INotificationDispatcher
{
    private readonly IEnumerable<INotificationProvider> _providers;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(IEnumerable<INotificationProvider> providers, ILogger<NotificationDispatcher> logger)
    {
        _providers = providers;
        _logger = logger;
    }

    public async Task DispatchAsync(NotificationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogInformation("Dispatching notification to {ProviderCount} providers", _providers.Count());

        var tasks = _providers.Select(async provider =>
        {
            try
            {
                await provider.SendAsync(request, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Failed to send notification via provider {ProviderName}", provider.ProviderName);
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}

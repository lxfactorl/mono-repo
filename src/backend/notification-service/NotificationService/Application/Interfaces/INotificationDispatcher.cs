using NotificationService.Domain.Models;

namespace NotificationService.Application.Interfaces;

/// <summary>
/// Orchestrates the dispatching of notifications to all registered providers.
/// </summary>
public interface INotificationDispatcher
{
    Task DispatchAsync(NotificationRequest request, CancellationToken cancellationToken = default);
}

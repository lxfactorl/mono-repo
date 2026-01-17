using NotificationService.Domain.Models;

namespace NotificationService.Domain.Interfaces;

/// <summary>
/// Defines a provider for sending notifications.
/// </summary>
internal interface INotificationProvider
{
    string ProviderName { get; }
    Task SendAsync(NotificationRequest request, CancellationToken cancellationToken = default);
}

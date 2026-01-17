namespace NotificationService.Domain.Models;

/// <summary>
/// Represents a notification request within the domain.
/// </summary>
internal record NotificationRequest(
    string Recipient,
    string Message,
    string? Subject = null,
    IDictionary<string, string>? Properties = null);

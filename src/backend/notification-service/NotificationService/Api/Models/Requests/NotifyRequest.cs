using System.ComponentModel.DataAnnotations;

namespace NotificationService.Api.Models.Requests;

/// <summary>
/// API request model for sending a notification.
/// </summary>
public record NotifyRequest(
    [property: Required][property: EmailAddress] string Recipient,
    [property: Required][property: MinLength(1)] string Message,
    string? Subject = null,
    IDictionary<string, string>? Properties = null);

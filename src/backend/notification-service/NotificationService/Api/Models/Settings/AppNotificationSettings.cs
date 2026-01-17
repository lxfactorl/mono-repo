using System.ComponentModel.DataAnnotations;

namespace NotificationService.Api.Models.Settings;

internal class AppNotificationSettings
{
    [Required]
    public string ServiceName { get; init; } = "NotificationService";

    [Range(1, 100)]
    public int MaxRetryAttempts { get; init; } = 3;
}

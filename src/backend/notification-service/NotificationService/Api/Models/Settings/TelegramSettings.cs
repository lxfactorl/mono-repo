using System.ComponentModel.DataAnnotations;

namespace NotificationService.Api.Models.Settings;

internal sealed class TelegramSettings
{
    [Required]
    public string BotToken { get; set; } = string.Empty;

    [Required]
    public string ChatId { get; set; } = string.Empty;
}

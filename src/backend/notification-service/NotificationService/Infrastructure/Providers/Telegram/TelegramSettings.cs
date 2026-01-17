using System.ComponentModel.DataAnnotations;

namespace NotificationService.Infrastructure.Providers.Telegram;

public class TelegramSettings
{
    [Required]
    public string BotToken { get; set; } = string.Empty;

    [Required]
    public string ChatId { get; set; } = string.Empty;
}

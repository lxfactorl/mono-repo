using System.ComponentModel.DataAnnotations;

namespace NotificationService.Api.Models.Settings;

internal sealed class TelegramSettings
{
    [Required(AllowEmptyStrings = false)]
    public string BotToken { get; set; } = string.Empty;

    [Required(AllowEmptyStrings = false)]
    public string ChatId { get; set; } = string.Empty;
}

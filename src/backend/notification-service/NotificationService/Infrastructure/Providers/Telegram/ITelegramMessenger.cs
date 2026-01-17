using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Providers.Telegram;

/// <summary>
/// Internal port for sending messages to Telegram.
/// Abstracts the concrete implementation for TDD.
/// </summary>
public interface ITelegramMessenger
{
    Task SendTextMessageAsync(string chatId, string message, CancellationToken cancellationToken);
}

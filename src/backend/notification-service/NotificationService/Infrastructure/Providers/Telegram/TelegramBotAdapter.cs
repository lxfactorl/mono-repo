using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotificationService.Infrastructure.Providers.Telegram;

/// <summary>
/// Adapter that implements ITelegramMessenger using the real Telegram.Bot client.
/// This is the "outside" adapter that connects to the real Telegram API.
/// </summary>
internal sealed class TelegramBotAdapter : ITelegramMessenger
{
    private readonly ITelegramBotClient _client;

    public TelegramBotAdapter(ITelegramBotClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task SendTextMessageAsync(string chatId, string message, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(chatId);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        try
        {
            var telegramChatId = ParseChatId(chatId);
            var escapedMessage = TelegramFormatter.EscapeMarkdownV2(message);
            await _client.SendMessage(
                telegramChatId,
                escapedMessage,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }
        catch (ApiRequestException ex)
        {
            throw new TelegramMessengerException($"Telegram API error: {ex.Message}", ex);
        }
    }

    private static ChatId ParseChatId(string chatId)
    {
        // Try parsing as numeric ID first
        if (long.TryParse(chatId, out var numericId))
        {
            return new ChatId(numericId);
        }

        // Otherwise treat as string key (username/channel)
        // Pass directly to ChatId constructor, letting it handle '@' or other formats
        return new ChatId(chatId);
    }
}

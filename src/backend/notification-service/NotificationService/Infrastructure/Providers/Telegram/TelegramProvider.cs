using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Providers.Telegram;

internal class TelegramProvider : INotificationProvider
{
    private readonly ITelegramMessenger _messenger;
    private readonly TelegramSettings _settings;
    private readonly ILogger<TelegramProvider> _logger;

    public TelegramProvider(
        ITelegramMessenger messenger,
        IOptions<TelegramSettings> settings,
        ILogger<TelegramProvider> logger)
    {
        _messenger = messenger;
        _settings = settings.Value;
        _logger = logger;
    }

    public string ProviderName => "Telegram";

    public async Task SendAsync(NotificationRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.BotToken))
            throw new ArgumentException("BotToken cannot be empty");

        if (string.IsNullOrWhiteSpace(_settings.ChatId))
            throw new ArgumentException("ChatId cannot be empty");

        try
        {
            await _messenger.SendTextMessageAsync(_settings.ChatId, request.Message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification via Telegram");
            throw;
        }
    }
}

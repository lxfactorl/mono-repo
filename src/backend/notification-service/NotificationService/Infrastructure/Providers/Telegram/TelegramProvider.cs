using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using NotificationService.Api.Models.Settings;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;

namespace NotificationService.Infrastructure.Providers.Telegram;

internal sealed class TelegramProvider : INotificationProvider
{
    private readonly ITelegramMessenger _messenger;
    private readonly TelegramSettings _settings;
    private readonly ILogger<TelegramProvider> _logger;

    public TelegramProvider(
        ITelegramMessenger messenger,
        TelegramSettings settings,
        ILogger<TelegramProvider> logger)
    {
        _messenger = messenger;
        _settings = settings;
        _logger = logger;
    }

    public string ProviderName => "Telegram";

    public async Task SendAsync(NotificationRequest request, CancellationToken cancellationToken = default)
    {
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

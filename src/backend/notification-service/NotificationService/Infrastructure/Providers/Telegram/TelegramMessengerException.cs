using System;

namespace NotificationService.Infrastructure.Providers.Telegram;

/// <summary>
/// Domain exception for Telegram messaging failures.
/// Wraps underlying API exceptions to provide a clean domain boundary.
/// </summary>
internal sealed class TelegramMessengerException : Exception
{
    public TelegramMessengerException()
    {
    }

    public TelegramMessengerException(string message) : base(message)
    {
    }

    public TelegramMessengerException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

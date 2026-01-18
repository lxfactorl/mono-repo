using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NotificationService.Api.Models.Settings;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Providers.Telegram;
using NSubstitute;
using Xunit;

namespace NotificationService.Tests.Unit.Infrastructure.Providers.Telegram;

public class TelegramProviderTests
{
    private readonly ITelegramMessenger _mockMessenger;
    private readonly ILogger<TelegramProvider> _mockLogger;

    public TelegramProviderTests()
    {
        _mockMessenger = Substitute.For<ITelegramMessenger>();
        _mockLogger = Substitute.For<ILogger<TelegramProvider>>();
    }

    [Fact]
    public async Task SendAsync_ShouldCallMessenger_WhenRequestIsValid()
    {
        // Arrange
        var settings = new TelegramSettings { BotToken = "token", ChatId = "chat-id" };
        var provider = new TelegramProvider(_mockMessenger, settings, _mockLogger);
        var request = new NotificationRequest("any", "Hello");

        // Act
        await provider.SendAsync(request);

        // Assert
        await _mockMessenger.Received(1).SendTextMessageAsync(
            "chat-id",
            "Hello",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_ShouldLogAndBubbleException_WhenMessengerFails()
    {
        // Arrange
        var settings = new TelegramSettings { BotToken = "token", ChatId = "chat-id" };
        var provider = new TelegramProvider(_mockMessenger, settings, _mockLogger);
        var request = new NotificationRequest("any", "Hello");

        _mockMessenger.SendTextMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new Exception("API Error")));

        // Act
        Func<Task> act = async () => await provider.SendAsync(request);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("API Error");
    }
}

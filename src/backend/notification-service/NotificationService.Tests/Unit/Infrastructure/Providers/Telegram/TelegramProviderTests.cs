using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NotificationService.Infrastructure.Providers.Telegram;
using NSubstitute;
using Xunit;

namespace NotificationService.Tests.Unit.Infrastructure.Providers.Telegram;

public class TelegramProviderTests
{
    private readonly ITelegramMessenger _messenger;
    private readonly ILogger<TelegramProvider> _logger;

    public TelegramProviderTests()
    {
        _messenger = Substitute.For<ITelegramMessenger>();
        _logger = Substitute.For<ILogger<TelegramProvider>>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task SendAsync_ShouldThrow_WhenBotTokenIsInvalid(string? invalidToken)
    {
        // Arrange
        var settings = Options.Create(new TelegramSettings { BotToken = invalidToken!, ChatId = "123" });
        var provider = new TelegramProvider(_messenger, settings, _logger);
        var request = new Domain.Models.NotificationRequest("recipient", "Hello");

        // Act
        Func<Task> act = async () => await provider.SendAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*BotToken*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task SendAsync_ShouldThrow_WhenChatIdIsInvalid(string? invalidChatId)
    {
        // Arrange
        var settings = Options.Create(new TelegramSettings { BotToken = "123:ABC", ChatId = invalidChatId! });
        var provider = new TelegramProvider(_messenger, settings, _logger);
        var request = new Domain.Models.NotificationRequest("recipient", "Hello");

        // Act
        Func<Task> act = async () => await provider.SendAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*ChatId*");
    }
    [Fact]
    public async Task SendAsync_ShouldCallMessenger_WhenRequestIsValid()
    {
        // Arrange
        var settings = Options.Create(new TelegramSettings { BotToken = "123:ABC", ChatId = "999" });
        var provider = new TelegramProvider(_messenger, settings, _logger);
        var request = new Domain.Models.NotificationRequest("recipient", "Hello *World*");

        // Act
        await provider.SendAsync(request);

        // Assert
        await _messenger.Received(1).SendTextMessageAsync(
            "999",
            "Hello *World*",
            Arg.Any<System.Threading.CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_ShouldLogAndBubble_WhenMessengerFails()
    {
        // Arrange
        var settings = Options.Create(new TelegramSettings { BotToken = "123:ABC", ChatId = "999" });
        var provider = new TelegramProvider(_messenger, settings, _logger);
        var request = new Domain.Models.NotificationRequest("recipient", "Hello");
        var expectedEx = new System.Net.Http.HttpRequestException("Network Error");

        _messenger.When(m => m.SendTextMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<System.Threading.CancellationToken>()))
            .Do(x => throw expectedEx);

        // Act
        Func<Task> act = async () => await provider.SendAsync(request);

        // Assert
        await act.Should().ThrowAsync<System.Net.Http.HttpRequestException>()
            .WithMessage("Network Error");

        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Failed to send")),
            expectedEx,
            Arg.Any<Func<object, Exception?, string>>());
    }
}

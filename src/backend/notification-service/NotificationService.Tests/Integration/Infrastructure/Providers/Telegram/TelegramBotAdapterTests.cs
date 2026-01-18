using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotificationService.Api.Models.Settings;
using NotificationService.Infrastructure.Providers.Telegram;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Xunit;

namespace NotificationService.Tests.Integration.Infrastructure.Providers.Telegram;

public class TelegramBotAdapterTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _baseFactory;

    public TelegramBotAdapterTests(WebApplicationFactory<Program> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _baseFactory = factory;
    }

    private WebApplicationFactory<Program> CreateFactoryWithMockedClient(
        Dictionary<string, string?> configOverrides,
        ITelegramBotClient mockClient)
    {
        return _baseFactory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(configOverrides);
            });

            builder.ConfigureTestServices(services =>
            {
                // Replace ITelegramBotClient with mock
                services.RemoveAll<ITelegramBotClient>();
                services.AddSingleton(mockClient);

                // Explicitly register settings for the provider (required for TelegramBotAdapter if it uses them, 
                // but adapter only uses client. Actually TelegramProvider uses settings.)
                services.AddSingleton(sp =>
                {
                    var config = sp.GetRequiredService<IConfiguration>();
                    return new TelegramSettings
                    {
                        BotToken = config["Telegram:BotToken"] ?? "test-token",
                        ChatId = config["Telegram:ChatId"] ?? "test-chat-id"
                    };
                });

                // Ensure TelegramBotAdapter is registered
                services.RemoveAll<ITelegramMessenger>();
                services.AddScoped<ITelegramMessenger, TelegramBotAdapter>();
            });
        });
    }

    [Fact]
    public async Task SendTextMessageAsync_ShouldCallClient_WithCorrectParameters()
    {
        // Arrange
        var mockClient = Substitute.For<ITelegramBotClient>();

        // In Telegram.Bot v22, SendMessage is an extension that calls SendRequest
        // We must mock the underlying request
        mockClient.SendRequest(Arg.Any<SendMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new Message()));

        var config = new Dictionary<string, string?>
        {
            ["Telegram:BotToken"] = "test-token",
            ["Telegram:ChatId"] = "test-chat-id"
        };

        using var factory = CreateFactoryWithMockedClient(config, mockClient);
        using var scope = factory.Services.CreateScope();
        var adapter = scope.ServiceProvider.GetRequiredService<ITelegramMessenger>();

        // Act
        await adapter.SendTextMessageAsync("12345", "Hello Telegram!", CancellationToken.None);

        // Assert
        // Verify that SendRequest was called with a SendMessageRequest containing correct data
        // Note: '!' is escaped by our TelegramFormatter
        await mockClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r =>
                r.ChatId.Identifier == 12345 &&
                r.Text == "Hello Telegram\\!" &&
                r.ParseMode == ParseMode.MarkdownV2),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendTextMessageAsync_ShouldThrowTelegramException_WhenApiRequestExceptionOccurs()
    {
        // Arrange
        var mockClient = Substitute.For<ITelegramBotClient>();
        // In Telegram.Bot v22, SendMessage is an extension that calls SendRequest
        mockClient.SendRequest(Arg.Any<SendMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Message>(new ApiRequestException("Bot token is invalid")));

        var config = new Dictionary<string, string?>
        {
            ["Telegram:BotToken"] = "invalid-token",
            ["Telegram:ChatId"] = "test-chat-id"
        };

        using var factory = CreateFactoryWithMockedClient(config, mockClient);
        using var scope = factory.Services.CreateScope();
        var adapter = scope.ServiceProvider.GetRequiredService<ITelegramMessenger>();

        // Act
        Func<Task> act = async () => await adapter.SendTextMessageAsync("12345", "Hello!", CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<TelegramMessengerException>()
            .WithMessage("*Telegram API*")
            .Where(e => e.InnerException is ApiRequestException);
    }

    [Fact]
    public async Task SendTextMessageAsync_ShouldHandleStringChatId_WithUsernameFormat()
    {
        // Arrange
        var mockClient = Substitute.For<ITelegramBotClient>();
        mockClient.SendRequest(Arg.Any<SendMessageRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(new Message()));

        var config = new Dictionary<string, string?>
        {
            ["Telegram:BotToken"] = "test-token",
            ["Telegram:ChatId"] = "@mychannel"
        };

        using var factory = CreateFactoryWithMockedClient(config, mockClient);
        using var scope = factory.Services.CreateScope();
        var adapter = scope.ServiceProvider.GetRequiredService<ITelegramMessenger>();

        // Act
        await adapter.SendTextMessageAsync("@mychannel", "Channel message", CancellationToken.None);

        // Assert
        await mockClient.Received(1).SendRequest(
            Arg.Is<SendMessageRequest>(r =>
                r.ChatId.Username == "@mychannel" &&
                r.Text == "Channel message" &&
                r.ParseMode == ParseMode.MarkdownV2),
            Arg.Any<CancellationToken>());
    }
}

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Providers.Telegram;
using NSubstitute;
using Xunit;

namespace NotificationService.Tests.Integration.Infrastructure.Providers.Telegram;

public class TelegramProviderTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _baseFactory;

    public TelegramProviderTests(WebApplicationFactory<Program> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _baseFactory = factory;
    }

    private WebApplicationFactory<Program> CreateFactoryWithConfig(
        Dictionary<string, string?> configOverrides,
        ITelegramMessenger? mockMessenger = null)
    {
        return _baseFactory.WithWebHostBuilder(builder =>
        {
            // Use in-memory configuration
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(configOverrides);
            });

            builder.ConfigureTestServices(services =>
            {
                // Inject mocked ITelegramMessenger
                if (mockMessenger != null)
                {
                    services.RemoveAll<ITelegramMessenger>();
                    services.AddSingleton(mockMessenger);
                }

                // Explicitly register TelegramProvider for testing (not in default DI until Phase 2)
                services.AddScoped<TelegramProvider>();
            });
        });
    }

    [Fact]
    public async Task SendAsync_ShouldCallMessenger_WhenRequestIsValid()
    {
        // Arrange
        var mockMessenger = Substitute.For<ITelegramMessenger>();
        var config = new Dictionary<string, string?>
        {
            ["Telegram:BotToken"] = "test-token",
            ["Telegram:ChatId"] = "test-chat-id"
        };

        using var factory = CreateFactoryWithConfig(config, mockMessenger);
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<TelegramProvider>();
        var request = new NotificationRequest("recipient", "Hello Integration");

        // Act
        await provider.SendAsync(request);

        // Assert
        await mockMessenger.Received(1).SendTextMessageAsync(
            "test-chat-id",
            "Hello Integration",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_ShouldThrow_WhenBotTokenIsEmpty()
    {
        // Arrange
        var mockMessenger = Substitute.For<ITelegramMessenger>();
        var config = new Dictionary<string, string?>
        {
            ["Telegram:BotToken"] = "",  // Invalid
            ["Telegram:ChatId"] = "valid-chat-id"
        };

        using var factory = CreateFactoryWithConfig(config, mockMessenger);
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<TelegramProvider>();
        var request = new NotificationRequest("recipient", "Hello");

        // Act
        Func<Task> act = async () => await provider.SendAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*BotToken*");
    }

    [Fact]
    public async Task SendAsync_ShouldThrow_WhenChatIdIsEmpty()
    {
        // Arrange
        var mockMessenger = Substitute.For<ITelegramMessenger>();
        var config = new Dictionary<string, string?>
        {
            ["Telegram:BotToken"] = "valid-token",
            ["Telegram:ChatId"] = ""  // Invalid
        };

        using var factory = CreateFactoryWithConfig(config, mockMessenger);
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<TelegramProvider>();
        var request = new NotificationRequest("recipient", "Hello");

        // Act
        Func<Task> act = async () => await provider.SendAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*ChatId*");
    }

    [Fact]
    public async Task SendAsync_ShouldLogAndBubbleException_WhenMessengerFails()
    {
        // Arrange
        var failingMessenger = Substitute.For<ITelegramMessenger>();
        failingMessenger
            .When(m => m.SendTextMessageAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()))
            .Do(_ => throw new HttpRequestException("API Error"));

        var config = new Dictionary<string, string?>
        {
            ["Telegram:BotToken"] = "valid-token",
            ["Telegram:ChatId"] = "valid-chat-id"
        };

        using var factory = CreateFactoryWithConfig(config, failingMessenger);
        using var scope = factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<TelegramProvider>();
        var request = new NotificationRequest("recipient", "Hello");

        // Act
        Func<Task> act = async () => await provider.SendAsync(request);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage("API Error");
    }
}

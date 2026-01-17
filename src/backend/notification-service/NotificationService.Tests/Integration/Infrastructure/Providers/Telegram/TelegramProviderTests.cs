using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;
using NotificationService.Infrastructure.Providers.Telegram;
using NSubstitute;
using Xunit;

namespace NotificationService.Tests.Integration.Infrastructure.Providers.Telegram;

public class TelegramProviderTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITelegramMessenger _mockMessenger;

    public TelegramProviderTests(WebApplicationFactory<Program> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        _mockMessenger = Substitute.For<ITelegramMessenger>();
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Register the provider if it's conditional and relies on config?
                // Actually, if we want to test TelegramProvider specifically, we might need to force register it 
                // if it's not registered by default when config is missing.
                // But for now, let's assume valid config injection for the test.

                services.Configure<TelegramSettings>(settings =>
                {
                    settings.BotToken = "test-token";
                    settings.ChatId = "test-chat-id";
                });

                // Replace the internal messenger with our mock
                services.RemoveAll<ITelegramMessenger>();
                services.AddSingleton(_mockMessenger);

                // Ensure TelegramProvider is registered (it might be conditional in Program.cs, 
                // but we can register it explicitly for the test context if needed, 
                // or rely on the app's boolean logic if we set settings correctly).
                // Let's force register it to ensure we are testing the class itself.
                services.AddScoped<TelegramProvider>();
            });
        });
    }

    [Fact]
    public async Task SendAsync_ShouldCallMessenger_WhenRequestIsValid()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<TelegramProvider>();
        var request = new NotificationRequest("recipient", "Hello Integration");

        // Act
        await provider.SendAsync(request);

        // Assert
        await _mockMessenger.Received(1).SendTextMessageAsync(
            "test-chat-id",
            "Hello Integration",
            Arg.Any<System.Threading.CancellationToken>());
    }

    [Fact]
    public async Task SendAsync_ShouldThrow_WhenSettingsAreInvalid()
    {
        // Arrange
        var badConfigFactory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.Configure<TelegramSettings>(settings =>
                {
                    settings.BotToken = ""; // Invalid
                    settings.ChatId = "valid";
                });
                services.AddScoped<TelegramProvider>();
                services.AddSingleton(Substitute.For<ITelegramMessenger>());
            });
        });

        using var scope = badConfigFactory.Services.CreateScope();
        var provider = scope.ServiceProvider.GetRequiredService<TelegramProvider>();
        var request = new NotificationRequest("recipient", "Hello");

        // Act
        Func<Task> act = async () => await provider.SendAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*BotToken*");
    }
}

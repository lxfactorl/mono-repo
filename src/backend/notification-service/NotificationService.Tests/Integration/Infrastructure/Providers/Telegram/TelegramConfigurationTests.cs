using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NotificationService.Api.Models.Settings;
using NotificationService.Infrastructure.Common;
using Xunit;

namespace NotificationService.Tests.Integration.Infrastructure.Providers.Telegram;

public class TelegramConfigurationTests
{
    [Fact]
    public void AddValidatedOptions_ShouldThrowOnStart_WhenConfigIsInvalid()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string?>
        {
            ["Telegram:BotToken"] = "", // Invalid
            ["Telegram:ChatId"] = ""
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();

        // Register
        services.AddValidatedOptions<TelegramSettings>(configuration, "Telegram");
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert
        // The validator is triggered when the options are resolved.
        // ValidateOnStart means it validates when the host starts, or when IOptions is resolved?
        // Actually ValidateOnStart() validates when the HostedService starts OR when we resolve the options.
        // Let's resolve valid options to see.
        var action = () => serviceProvider.GetRequiredService<TelegramSettings>();

        action.Should().Throw<OptionsValidationException>();
    }

    [Fact]
    public void AddValidatedOptions_ShouldSucceed_WhenConfigIsValid()
    {
        // Arrange
        var services = new ServiceCollection();
        var configData = new Dictionary<string, string?>
        {
            ["Telegram:BotToken"] = "valid_token",
            ["Telegram:ChatId"] = "valid_chat_id"
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();

        // Register
        services.AddValidatedOptions<TelegramSettings>(configuration, "Telegram");
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var settings = serviceProvider.GetRequiredService<TelegramSettings>();

        // Assert
        settings.BotToken.Should().Be("valid_token");
        settings.ChatId.Should().Be("valid_chat_id");
    }
}

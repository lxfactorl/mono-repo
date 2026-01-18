using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NotificationService.Api.Models.Settings;
using Xunit;

namespace NotificationService.Tests.Integration.Infrastructure.Providers.Telegram;

public class TelegramSettingsBindingTests
{
    [Fact]
    public void TelegramSettings_ShouldBindCorrectly_FromConfiguration()
    {
        // Arrange
        var configData = new Dictionary<string, string?>
        {
            ["Telegram:BotToken"] = "123456789:ABCdefGHIjklMNOpqrSTUvwxYZ",
            ["Telegram:ChatId"] = "987654321"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        var settings = new TelegramSettings();

        // Act
        configuration.GetSection("Telegram").Bind(settings);

        // Assert
        settings.BotToken.Should().Be("123456789:ABCdefGHIjklMNOpqrSTUvwxYZ");
        settings.ChatId.Should().Be("987654321");
    }

    [Fact]
    public void TelegramSettings_ShouldHaveEmptyDefaults_WhenNotConfigured()
    {
        // Arrange
        var settings = new TelegramSettings(); // Default constructor

        // Assert
        settings.BotToken.Should().BeEmpty();
        settings.ChatId.Should().BeEmpty();
    }

    [Fact]
    public void TelegramSettings_ShouldFailValidation_WhenBotTokenIsMissing()
    {
        // Arrange
        var settings = new TelegramSettings
        {
            BotToken = "", // Invalid
            ChatId = "123456"
        };

        // Act
        var context = new ValidationContext(settings);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains(nameof(TelegramSettings.BotToken)));
    }

    [Fact]
    public void TelegramSettings_ShouldFailValidation_WhenChatIdIsMissing()
    {
        // Arrange
        var settings = new TelegramSettings
        {
            BotToken = "valid-token",
            ChatId = "" // Invalid
        };

        // Act
        var context = new ValidationContext(settings);
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(settings, context, results, validateAllProperties: true);

        // Assert
        isValid.Should().BeFalse();
        results.Should().Contain(r => r.MemberNames.Contains(nameof(TelegramSettings.ChatId)));
    }
}

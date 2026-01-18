using FluentAssertions;
using NotificationService.Infrastructure.Providers.Telegram;
using Xunit;

namespace NotificationService.Tests.Unit.Infrastructure.Providers.Telegram;

public class TelegramFormatterTests
{
    [Theory]
    [InlineData("Hello", "Hello")]
    [InlineData("Hello.", "Hello\\.")]
    [InlineData("Hello!", "Hello\\!")]
    [InlineData("Version 1.0", "Version 1\\.0")]
    [InlineData("Dashes-And-Pluses+", "Dashes\\-And\\-Pluses\\+")]
    [InlineData("Mixed symbols # >", "Mixed symbols \\# \\>")]
    [InlineData("Keep *Bold*", "Keep *Bold*")]
    [InlineData("Keep _Italic_", "Keep _Italic_")]
    [InlineData("Keep `Code`", "Keep `Code`")]
    [InlineData("Keep ||Spoiler||", "Keep ||Spoiler||")]
    public void EscapeMarkdownV2_ShouldEscapePunctuation_ButKeepFormatting(string input, string expected)
    {
        // Act
        var result = TelegramFormatter.EscapeMarkdownV2(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void EscapeMarkdownV2_ShouldHandleNullOrEmpty()
    {
        TelegramFormatter.EscapeMarkdownV2(null).Should().BeNull();
        TelegramFormatter.EscapeMarkdownV2(string.Empty).Should().BeEmpty();
    }
}

using System.Text;

namespace NotificationService.Infrastructure.Providers.Telegram;

internal static class TelegramFormatter
{
    /// <summary>
    /// Escapes special characters for Telegram MarkdownV2.
    /// Note: This is a basic implementation. It avoids escaping characters commonly used for 
    /// deliberate formatting in this project (*, _, |, `, [, ], (, )).
    /// </summary>
    public static string EscapeMarkdownV2(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        // Characters that MUST be escaped in MarkdownV2 if not part of a tag
        // _, *, [, ], (, ), ~, `, >, #, +, -, =, |, {, }, ., !
        // We skip escaping the core ones we want to support for "Auto-Markdown"
        char[] specialChars = { '~', '>', '#', '+', '-', '=', '{', '}', '.', '!' };

        var sb = new StringBuilder(text.Length);
        foreach (var c in text)
        {
            if (Array.IndexOf(specialChars, c) >= 0)
            {
                sb.Append('\\');
            }
            sb.Append(c);
        }

        return sb.ToString();
    }
}

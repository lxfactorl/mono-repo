namespace NotificationService.Api.Models.Responses;

/// <summary>
/// Detailed exception information for debugging purposes.
/// </summary>
internal class ExceptionDetails
{
    /// <summary>
    /// Exception type name.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Exception message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Stack trace (optional, for deep debugging).
    /// </summary>
    public string? StackTrace { get; init; }
}

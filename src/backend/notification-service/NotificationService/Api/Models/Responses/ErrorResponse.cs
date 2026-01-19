namespace NotificationService.Api.Models.Responses;

/// <summary>
/// Standard error response model returned by the API for all error scenarios.
/// </summary>
internal class ErrorResponse
{
    /// <summary>
    /// HTTP status code.
    /// </summary>
    public required int StatusCode { get; init; }

    /// <summary>
    /// User-friendly error message.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Detailed exception information (only included in Development environment).
    /// </summary>
    public ExceptionDetails? Exception { get; init; }
}

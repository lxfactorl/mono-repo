namespace NotificationService.Api.Models.Responses;

/// &lt;summary&gt;
/// Standard error response model returned by the API for all error scenarios.
/// &lt;/summary&gt;
public class ErrorResponse
{
    /// &lt;summary&gt;
    /// HTTP status code.
    /// &lt;/summary&gt;
    public required int StatusCode { get; init; }

    /// &lt;summary&gt;
    /// User-friendly error message.
    /// &lt;/summary&gt;
    public required string Message { get; init; }

    /// &lt;summary&gt;
    /// Detailed exception information (only included in Development environment).
    /// &lt;/summary&gt;
    public ExceptionDetails? Exception { get; init; }
}

/// &lt;summary&gt;
/// Detailed exception information for debugging purposes.
/// &lt;/summary&gt;
public class ExceptionDetails
{
    /// &lt;summary&gt;
    /// Exception type name.
    /// &lt;/summary&gt;
    public required string Type { get; init; }

    /// &lt;summary&gt;
    /// Exception message.
    /// &lt;/summary&gt;
    public required string Message { get; init; }

    /// &lt;summary&gt;
    /// Stack trace (optional, for deep debugging).
    /// &lt;/summary&gt;
    public string? StackTrace { get; init; }
}

namespace NotificationService.Infrastructure.Middleware;

/// <summary>
/// Configuration options for the global exception handler.
/// </summary>
public class GlobalExceptionHandlerOptions
{
    /// <summary>
    /// Custom exception mapper that allows clients to define their own exception-to-error mapping logic.
    /// If provided, this will be called before the default mapper.
    /// Return null to fall back to the default mapping.
    /// </summary>
    /// <example>
    /// Example: Map multiple custom exception types using a switch expression:
    /// <code>
    /// CustomExceptionMapper = exception => exception switch
    /// {
    ///     ValidationException => (400, "Validation failed"),
    ///     NotFoundException => (404, "Resource not found"),
    ///     BusinessRuleException => (422, "Business rule violation"),
    ///     _ => null  // Fall back to default mapping
    /// }
    /// </code>
    /// </example>
    public Func<Exception, (int StatusCode, string Message)?>? CustomExceptionMapper { get; set; }
}

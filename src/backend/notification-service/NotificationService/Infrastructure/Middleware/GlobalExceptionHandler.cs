using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotificationService.Api.Models.Responses;

namespace NotificationService.Infrastructure.Middleware;

/// <summary>
/// Global exception handler that catches all unhandled exceptions and returns structured error responses.
/// In Development: includes detailed exception information.
/// In Production: returns generic error messages to avoid leaking sensitive information.
/// </summary>
internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _environment;
    private readonly GlobalExceptionHandlerOptions _options;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IHostEnvironment environment,
        GlobalExceptionHandlerOptions? options = null)
    {
        _logger = logger;
        _environment = environment;
        _options = options ?? new GlobalExceptionHandlerOptions();
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Log the exception with full details
        _logger.LogError(
            exception,
            "Unhandled exception occurred: {ExceptionType} - {Message}",
            exception.GetType().Name,
            exception.Message);

        // Determine status code and message based on exception type
        var (statusCode, message) = GetErrorDetails(exception);

        // Build error response
        var errorResponse = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            Exception = _environment.IsDevelopment()
                ? new ExceptionDetails
                {
                    Type = exception.GetType().Name,
                    Message = exception.Message,
                    StackTrace = exception.StackTrace
                }
                : null
        };

        // Set response status code and content type
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        // Write JSON response
        await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);

        // Return true to indicate the exception has been handled
        return true;
    }

    /// <summary>
    /// Maps exception types to appropriate HTTP status codes and user-friendly messages.
    /// First tries the custom mapper (if configured), then falls back to default mapping.
    /// </summary>
    [ExcludeFromCodeCoverage] // Simple mapping logic, covered by integration tests
    private (int StatusCode, string Message) GetErrorDetails(Exception exception)
    {
        // Try custom mapper first
        if (_options.CustomExceptionMapper != null)
        {
            var customResult = _options.CustomExceptionMapper(exception);
            if (customResult.HasValue)
            {
                return customResult.Value;
            }
        }

        // Fall back to default mapping
        return exception switch
        {
            ArgumentException or ArgumentNullException => (400, "Invalid request. Please check your input."),
            KeyNotFoundException => (404, "The requested resource was not found."),
            InvalidOperationException => (409, "The operation could not be completed due to a conflict."),
            UnauthorizedAccessException => (403, "You do not have permission to perform this action."),
            TimeoutException => (503, "The service is temporarily unavailable. Please try again later."),
            _ => (500, "An internal server error occurred. Please try again later.")
        };
    }
}

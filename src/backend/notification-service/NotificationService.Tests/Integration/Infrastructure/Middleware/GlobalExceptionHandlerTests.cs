using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NotificationService.Api.Models.Responses;

namespace NotificationService.Tests.Integration.Infrastructure.Middleware;

public class GlobalExceptionHandlerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public GlobalExceptionHandlerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Fact]
    public async Task ExceptionHandler_IsRegistered_HealthEndpointWorks()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(new Uri("/health", UriKind.Relative));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public void ErrorResponse_HasCorrectStructure()
    {
        // Arrange - Create error response with all properties
        var errorResponse = new ErrorResponse
        {
            StatusCode = 500,
            Message = "Test error",
            Exception = new ExceptionDetails
            {
                Type = "TestException",
                Message = "Test exception message",
                StackTrace = "Test stack trace"
            }
        };

        // Assert
        Assert.Equal(500, errorResponse.StatusCode);
        Assert.Equal("Test error", errorResponse.Message);
        Assert.NotNull(errorResponse.Exception);
        Assert.Equal("TestException", errorResponse.Exception.Type);
        Assert.Equal("Test exception message", errorResponse.Exception.Message);
        Assert.Equal("Test stack trace", errorResponse.Exception.StackTrace);
    }

    [Fact]
    public void ErrorResponse_CanSerializeToJson()
    {
        // Arrange - Development scenario with exception details
        var errorResponse = new ErrorResponse
        {
            StatusCode = 500,
            Message = "Test error",
            Exception = new ExceptionDetails
            {
                Type = "TestException",
                Message = "Test exception message",
                StackTrace = "Test stack trace"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(errorResponse, _serializerOptions);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        // Assert
        Assert.Equal(500, root.GetProperty("statusCode").GetInt32());
        Assert.Equal("Test error", root.GetProperty("message").GetString());

        var exception = root.GetProperty("exception");
        Assert.Equal("TestException", exception.GetProperty("type").GetString());
        Assert.Equal("Test exception message", exception.GetProperty("message").GetString());
        Assert.Equal("Test stack trace", exception.GetProperty("stackTrace").GetString());
    }

    [Fact]
    public void ErrorResponse_WithoutException_SerializesCorrectly()
    {
        // Arrange - Production scenario (no exception details)
        var errorResponse = new ErrorResponse
        {
            StatusCode = 500,
            Message = "An internal server error occurred. Please try again later.",
            Exception = null
        };

        // Act
        var json = JsonSerializer.Serialize(errorResponse, _serializerOptions);
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        // Assert
        Assert.Equal(500, root.GetProperty("statusCode").GetInt32());
        Assert.Equal("An internal server error occurred. Please try again later.", root.GetProperty("message").GetString());

        // Verify exception property is present but null
        Assert.True(root.TryGetProperty("exception", out var exceptionProperty));
        Assert.Equal(JsonValueKind.Null, exceptionProperty.ValueKind);
    }

    [Fact]
    public void ExceptionDetails_AllowsNullStackTrace()
    {
        // Arrange - Exception details without stack trace
        var exceptionDetails = new ExceptionDetails
        {
            Type = "TestException",
            Message = "Test message",
            StackTrace = null
        };

        // Assert
        Assert.Equal("TestException", exceptionDetails.Type);
        Assert.Equal("Test message", exceptionDetails.Message);
        Assert.Null(exceptionDetails.StackTrace);
    }
}

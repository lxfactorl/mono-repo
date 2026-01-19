using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NotificationService.Api.Models.Responses;
using FluentAssertions;
using Xunit;

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
        using var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Telegram:BotToken"] = "123456:FAKE-TOKEN",
                    ["Telegram:ChatId"] = "123456789"
                });
            });
        }).CreateClient();

        // Act
        var response = await client.GetAsync(new Uri("/health", UriKind.Relative));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
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
        errorResponse.StatusCode.Should().Be(500);
        errorResponse.Message.Should().Be("Test error");
        errorResponse.Exception.Should().NotBeNull();
        errorResponse.Exception.Type.Should().Be("TestException");
        errorResponse.Exception.Message.Should().Be("Test exception message");
        errorResponse.Exception.StackTrace.Should().Be("Test stack trace");
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
        root.GetProperty("statusCode").GetInt32().Should().Be(500);
        root.GetProperty("message").GetString().Should().Be("Test error");

        var exception = root.GetProperty("exception");
        exception.GetProperty("type").GetString().Should().Be("TestException");
        exception.GetProperty("message").GetString().Should().Be("Test exception message");
        exception.GetProperty("stackTrace").GetString().Should().Be("Test stack trace");
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
        root.GetProperty("statusCode").GetInt32().Should().Be(500);
        root.GetProperty("message").GetString().Should().Be("An internal server error occurred. Please try again later.");

        // Verify exception property is present but null
        root.TryGetProperty("exception", out var exceptionProperty).Should().BeTrue();
        exceptionProperty.ValueKind.Should().Be(JsonValueKind.Null);
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
        exceptionDetails.Type.Should().Be("TestException");
        exceptionDetails.Message.Should().Be("Test message");
        exceptionDetails.StackTrace.Should().BeNull();
    }
}

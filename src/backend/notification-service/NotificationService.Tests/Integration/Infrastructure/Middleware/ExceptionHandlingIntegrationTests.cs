using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NotificationService.Api.Models.Requests;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Models;
using System.Net.Http.Json;
using NotificationService.Infrastructure.Middleware;
using NSubstitute;
using Xunit;
using FluentAssertions;

namespace NotificationService.Tests.Integration.Infrastructure.Middleware;

public class ExceptionHandlingIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ExceptionHandlingIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HandleException_InDevelopment_ReturnsDetailedResponse()
    {
        // Arrange
        var mockDispatcher = Substitute.For<INotificationDispatcher>();
        var exceptionMessage = "Development error details";
        mockDispatcher.DispatchAsync(Arg.Any<NotificationRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new Exception(exceptionMessage)));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Telegram:BotToken"] = "123456:FAKE-TOKEN",
                    ["Telegram:ChatId"] = "123456789"
                });
            });
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<INotificationDispatcher>();
                services.AddScoped(_ => mockDispatcher);
            });
        }).CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/notify", new NotifyRequest("test@test.com", "msg"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        root.GetProperty("statusCode").GetInt32().Should().Be(500);
        root.GetProperty("message").GetString().Should().Be("An internal server error occurred. Please try again later.");

        // In Development, we expect an 'exception' property
        root.TryGetProperty("exception", out var exceptionProp).Should().BeTrue();
        exceptionProp.GetProperty("type").GetString().Should().Be("Exception");
        exceptionProp.GetProperty("message").GetString().Should().Be(exceptionMessage);
        exceptionProp.TryGetProperty("stackTrace", out _).Should().BeTrue();
    }

    [Fact]
    public async Task HandleException_InProduction_ReturnsGenericResponse()
    {
        // Arrange
        var mockDispatcher = Substitute.For<INotificationDispatcher>();
        mockDispatcher.DispatchAsync(Arg.Any<NotificationRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new Exception("Hidden details")));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Production");
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Telegram:BotToken"] = "123456:FAKE-TOKEN",
                    ["Telegram:ChatId"] = "123456789"
                });
            });
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<INotificationDispatcher>();
                services.AddScoped(_ => mockDispatcher);
            });
        }).CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/notify", new NotifyRequest("test@test.com", "msg"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        root.GetProperty("statusCode").GetInt32().Should().Be(500);
        root.GetProperty("message").GetString().Should().Be("An internal server error occurred. Please try again later.");

        // In Production, exception property should be null or missing
        if (root.TryGetProperty("exception", out var exceptionProp))
        {
            exceptionProp.ValueKind.Should().Be(JsonValueKind.Null);
        }
    }

    [Fact]
    public async Task HandleException_WithCustomMapping_ReturnsMappedResponse()
    {
        // Arrange
        var mockDispatcher = Substitute.For<INotificationDispatcher>();
        mockDispatcher.DispatchAsync(Arg.Any<NotificationRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException(new ArgumentException("Invalid argument provided")));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development"); // Using dev to see if exception details interfere, but mapping should take precedence for status/message
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Telegram:BotToken"] = "123456:FAKE-TOKEN",
                    ["Telegram:ChatId"] = "123456789"
                });
            });
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<INotificationDispatcher>();
                services.AddScoped(_ => mockDispatcher);

                // Configure custom mapping
                services.Replace(ServiceDescriptor.Singleton(new GlobalExceptionHandlerOptions
                {
                    CustomExceptionMapper = ex => ex is ArgumentException
                        ? (400, "Custom Bad Request Message")
                        : null
                }));
            });
        }).CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/notify", new NotifyRequest("test@test.com", "msg"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var json = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;

        root.GetProperty("statusCode").GetInt32().Should().Be(400);
        root.GetProperty("message").GetString().Should().Be("Custom Bad Request Message");

        // Even with custom mapping, dev mode might add details. Let's verify.
        root.TryGetProperty("exception", out var exceptionProp).Should().BeTrue();
        exceptionProp.GetProperty("type").GetString().Should().Be("ArgumentException");
    }
}

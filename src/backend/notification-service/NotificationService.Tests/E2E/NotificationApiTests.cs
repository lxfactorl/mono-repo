using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using NotificationService.Api.Models.Requests;
using Xunit;

namespace NotificationService.Tests.E2E;

public class NotificationApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NotificationApiTests(WebApplicationFactory<Program> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                // Provide dummy valid credentials to satisfy validation, even if we are not testing Telegram here
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Telegram:BotToken"] = "123456:FAKE-TOKEN",
                    ["Telegram:ChatId"] = "123456789"
                });
            });
        }).CreateClient();
    }

    [Fact]
    public async Task PostNotify_WithValidRequest_ReturnsAccepted()
    {
        // Arrange
        var request = new NotifyRequest("user@example.com", "Testing E2E notifications");

        // Act
        var response = await _client.PostAsJsonAsync("/notify", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Fact]
    public async Task PostNotify_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var request = new NotifyRequest("not-an-email", "Testing invalid request");

        // Act
        var response = await _client.PostAsJsonAsync("/notify", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostNotify_WithEmptyMessage_ReturnsBadRequest()
    {
        // Arrange
        var request = new NotifyRequest("user@example.com", "");

        // Act
        var response = await _client.PostAsJsonAsync("/notify", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

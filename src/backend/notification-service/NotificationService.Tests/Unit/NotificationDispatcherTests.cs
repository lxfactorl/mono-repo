using FluentAssertions;
using Microsoft.Extensions.Logging;
using NotificationService.Application.Services;
using NotificationService.Domain.Interfaces;
using NotificationService.Domain.Models;
using NSubstitute;
using Xunit;

namespace NotificationService.Tests.Unit;

public class NotificationDispatcherTests
{
    private readonly INotificationProvider _provider1 = Substitute.For<INotificationProvider>();
    private readonly INotificationProvider _provider2 = Substitute.For<INotificationProvider>();
    private readonly ILogger<NotificationDispatcher> _logger = Substitute.For<ILogger<NotificationDispatcher>>();
    private readonly NotificationDispatcher _sut;

    public NotificationDispatcherTests()
    {
        _sut = new NotificationDispatcher(new[] { _provider1, _provider2 }, _logger);
    }

    [Fact]
    public async Task DispatchAsync_ShouldCallAllProviders()
    {
        // Arrange
        var request = new NotificationRequest("test@example.com", "Hello");
        var ct = CancellationToken.None;

        // Act
        await _sut.DispatchAsync(request, ct);

        // Assert
        await _provider1.Received(1).SendAsync(request, ct);
        await _provider2.Received(1).SendAsync(request, ct);
    }

    [Fact]
    public async Task DispatchAsync_ShouldNotThrow_WhenProviderFails()
    {
        // Arrange
        var request = new NotificationRequest("test@example.com", "Hello");
        _provider1.When(x => x.SendAsync(Arg.Any<NotificationRequest>(), Arg.Any<CancellationToken>()))
                  .Do(x => throw new Exception("Provider failed"));

        // Act
        var act = () => _sut.DispatchAsync(request);

        // Assert
        await act.Should().NotThrowAsync();
        await _provider2.Received(1).SendAsync(request, Arg.Any<CancellationToken>());
    }

    [Fact]
    public void Gatekeeper_MustBlock_OnTestFailure()
    {
        // This test MUST fail and the gatekeeper MUST block merge
        Assert.Fail("GATEKEEPER TEST: Merge must be blocked by branch protection");
    }
}

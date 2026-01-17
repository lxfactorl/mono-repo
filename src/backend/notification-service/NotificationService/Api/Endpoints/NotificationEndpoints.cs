using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NotificationService.Api.Models.Requests;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace NotificationService.Api.Endpoints;

public static class NotificationEndpoints
{
    public static IEndpointRouteBuilder MapNotificationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapPost("/notify", async (
            [FromBody] NotifyRequest request,
            [FromServices] INotificationDispatcher dispatcher,
            CancellationToken ct) =>
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(dispatcher);

            var validationContext = new ValidationContext(request);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
            {
                return Results.BadRequest(validationResults.Select(r => new { r.MemberNames, r.ErrorMessage }));
            }

            var domainRequest = new NotificationRequest(
                request.Recipient,
                request.Message,
                request.Subject,
                request.Properties);

            await dispatcher.DispatchAsync(domainRequest, ct).ConfigureAwait(false);

            return Results.Accepted();
        })
        .WithName("SendNotification")
        .WithDescription("Sends a notification to all registered delivery channels.")
        .Produces(StatusCodes.Status202Accepted)
        .Produces(StatusCodes.Status400BadRequest);

        return endpoints;
    }
}

using ShopCore.Application.Common.Models;
using ShopCore.Application.Subscriptions.Commands.CreateSubscription;
using ShopCore.Application.Subscriptions.Commands.PauseSubscription;
using ShopCore.Application.Subscriptions.Commands.ResumeSubscription;
using ShopCore.Application.Subscriptions.Commands.SettleSubscription;
using ShopCore.Application.Subscriptions.Commands.UpdateSubscription;
using ShopCore.Application.Subscriptions.DTOs;
using ShopCore.Application.Subscriptions.Queries.GetMySubscriptions;
using ShopCore.Application.Subscriptions.Queries.GetSubscriptionById;
using ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptions;

namespace ShopCore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/v1/subscriptions
    [Authorize(Roles = "Customer,Vendor")]
    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> CreateSubscription(
    [FromBody] CreateSubscriptionCommand command)
    {
        var subscription = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetSubscriptionById),
            new { id = subscription.Id },
            subscription
        );
    }

    // GET /api/v1/subscriptions
    [Authorize(Roles = "Customer,Vendor")]
    [HttpGet]
    public async Task<ActionResult<PaginatedList<SubscriptionDto>>> GetMySubscriptions(
        [FromQuery] GetMySubscriptionsQuery query)
    {
        var subscriptions = await _mediator.Send(query);
        return Ok(subscriptions);
    }

    // GET /api/v1/subscriptions/{id}
    [Authorize(Roles = "Customer,Vendor")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SubscriptionDto>> GetSubscriptionById(
        [FromRoute] int id)
    {
        var subscription = await _mediator.Send(
            new GetSubscriptionByIdQuery(id)
        );
        return Ok(subscription);
    }

    // PATCH /api/v1/subscriptions/{id}
    [Authorize(Roles = "Customer,Vendor")]
    [HttpPatch("{id}")]
    public async Task<ActionResult<SubscriptionDto>> UpdateSubscription(
        [FromRoute] int id,
        [FromBody] UpdateSubscriptionCommand command)
    {
        command.SubscriptionId = id;

        var subscription = await _mediator.Send(command);
        return Ok(subscription);
    }

    // PATCH /api/v1/subscriptions/{id}/pause
    [HttpPatch("{id}/pause")]
    public async Task<IActionResult> PauseSubscription(
        [FromRoute] int id)
    {
        await _mediator.Send(new PauseSubscriptionCommand(id));
        return NoContent();
    }

    // PATCH /api/v1/subscriptions/{id}/resume
    [HttpPatch("{id}/resume")]
    public async Task<IActionResult> ResumeSubscription(
        [FromRoute] int id)
    {
        await _mediator.Send(new ResumeSubscriptionCommand(id));
        return NoContent();
    }

    // POST /api/v1/subscriptions/{id}/cancel
    [Authorize(Roles = "Customer,Vendor")]
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelSubscription(
        [FromRoute] int id)
    {

        await _mediator.Send(new CancelSubscriptionCommand(id));
        return NoContent();
    }

    // POST /api/v1/subscriptions/{id}/settle
    [HttpPost("{id}/settle")]
    public async Task<ActionResult<SubscriptionSettlementDto>> SettleSubscription(
        [FromRoute] int id)
    {
        await _mediator.Send(new SettleSubscriptionCommand(id));
        return NoContent();
    }

    // ----------------
    // Vendor endpoints
    // ----------------

    // GET /api/v1/vendors/me/subscriptions
    [Authorize(Roles = "Vendor")]
    [HttpGet("/api/v1/vendors/me/subscriptions")]
    public async Task<ActionResult<PaginatedList<SubscriptionDto>>> GetVendorSubscriptions(
        [FromQuery] GetVendorSubscriptionsQuery query)
    {
        var subscriptions = await _mediator.Send(query);
        return Ok(subscriptions);
    }
}

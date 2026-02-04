using ShopCore.Application.Deliveries.Commands.SkipDelivery;
using ShopCore.Application.Invoices.Commands.PayInvoice;
using ShopCore.Application.Invoices.DTOs;
using ShopCore.Application.Subscriptions.Commands.AddOneTimeItemToSubscription;
using ShopCore.Application.Subscriptions.Commands.CancelSubscription;
using ShopCore.Application.Subscriptions.Commands.ConvertToSubscription;
using ShopCore.Application.Subscriptions.Commands.CreateOneTimeDelivery;
using ShopCore.Application.Subscriptions.Commands.CreateSubscription;
using ShopCore.Application.Subscriptions.Commands.PauseSubscription;
using ShopCore.Application.Subscriptions.Commands.ResumeSubscription;
using ShopCore.Application.Subscriptions.Commands.SettleSubscription;
using ShopCore.Application.Subscriptions.Commands.UpdateSubscription;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Api.Controllers;

/// <summary>
/// Subscription creation and management actions.
/// Viewing subscriptions is at /api/v1/users/me/subscriptions
/// Vendor subscription management is at /api/v1/vendors/me/subscriptions
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/v1/subscriptions
    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> CreateSubscription(
        [FromBody] CreateSubscriptionCommand command)
    {
        var subscription = await _mediator.Send(command);
        return CreatedAtRoute(null, new { id = subscription.Id }, subscription);
    }

    // PATCH /api/v1/subscriptions/{id}
    [HttpPatch("{id:int}")]
    public async Task<ActionResult<SubscriptionDto>> UpdateSubscription(
        int id,
        [FromBody] UpdateSubscriptionCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("ID in the URL does not match the ID in the request body.");
        }
        var subscription = await _mediator.Send(command);
        return Ok(subscription);
    }

    // POST /api/v1/subscriptions/{id}/items
    [HttpPost("{id:int}/items")]
    public async Task<ActionResult<SubscriptionItemResultDto>> AddOneTimeItem(
        int id,
        [FromBody] AddOneTimeItemToSubscriptionCommand command)
    {
        var finalCommand = command with { SubscriptionId = id };
        var item = await _mediator.Send(finalCommand);
        return Created("", item);
    }

    // POST /api/v1/subscriptions/{id}/pause
    [HttpPost("{id:int}/pause")]
    public async Task<IActionResult> PauseSubscription(int id)
    {
        await _mediator.Send(new PauseSubscriptionCommand(id));
        return NoContent();
    }

    // POST /api/v1/subscriptions/{id}/resume
    [HttpPost("{id:int}/resume")]
    public async Task<IActionResult> ResumeSubscription(int id)
    {
        await _mediator.Send(new ResumeSubscriptionCommand(id));
        return NoContent();
    }

    // POST /api/v1/subscriptions/{id}/settle
    [HttpPost("{id:int}/settle")]
    public async Task<ActionResult<SubscriptionSettlementDto>> SettleSubscription(int id)
    {
        var settlement = await _mediator.Send(new SettleSubscriptionCommand(id));
        return Ok(settlement);
    }

    // DELETE /api/v1/subscriptions/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> CancelSubscription(int id)
    {
        await _mediator.Send(new CancelSubscriptionCommand(id));
        return NoContent();
    }

    // ==================
    // Delivery Actions
    // ==================

    // POST /api/v1/subscriptions/deliveries/{deliveryId}/skip
    [HttpPost("deliveries/{deliveryId:int}/skip")]
    public async Task<IActionResult> SkipDelivery(
        int deliveryId,
        [FromBody] SkipDeliveryCommand command)
    {
        var finalCommand = command with { Id = deliveryId };
        await _mediator.Send(finalCommand);
        return NoContent();
    }

    // ==================
    // Invoice Actions
    // ==================

    // POST /api/v1/subscriptions/invoices/{invoiceId}/pay
    [HttpPost("invoices/{invoiceId:int}/pay")]
    public async Task<IActionResult> PayInvoice(
        int invoiceId,
        [FromBody] PayInvoiceRequest request)
    {
        var command = new PayInvoiceCommand(invoiceId, request.PaymentMethod, request.PaymentTransactionId);
        await _mediator.Send(command);
        return NoContent();
    }

    // ==================
    // One-Time Delivery
    // ==================

    // POST /api/v1/subscriptions/one-time-delivery
    [HttpPost("one-time-delivery")]
    public async Task<ActionResult<OneTimeDeliveryDto>> CreateOneTimeDelivery(
        [FromBody] CreateOneTimeDeliveryCommand command)
    {
        var delivery = await _mediator.Send(command);
        return Created("", delivery);
    }

    // POST /api/v1/subscriptions/{id}/convert
    [HttpPost("{id:int}/convert")]
    public async Task<ActionResult<SubscriptionDto>> ConvertToRecurring(
        int id,
        [FromBody] ConvertToSubscriptionCommand command)
    {
        var finalCommand = command with { OneTimeSubscriptionId = id };
        var subscription = await _mediator.Send(finalCommand);
        return Ok(subscription);
    }
}

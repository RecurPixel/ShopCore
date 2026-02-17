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

    /// <summary>
    /// Creates a new subscription.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>SubscriptionDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> CreateSubscription(
        [FromBody] CreateSubscriptionCommand command)
    {
        var subscription = await _mediator.Send(command);
        return CreatedAtRoute(null, new { id = subscription.Id }, subscription);
    }

    /// <summary>
    /// Updates subscription.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>SubscriptionDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Adds one time item.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>SubscriptionItemResultDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("{id:int}/items")]
    public async Task<ActionResult<SubscriptionItemResultDto>> AddOneTimeItem(
        int id,
        [FromBody] AddOneTimeItemToSubscriptionCommand command)
    {
        var finalCommand = command with { SubscriptionId = id };
        var item = await _mediator.Send(finalCommand);
        return Created("", item);
    }

    /// <summary>
    /// Pauses subscription.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("{id:int}/pause")]
    public async Task<IActionResult> PauseSubscription(int id)
    {
        await _mediator.Send(new PauseSubscriptionCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Resumes subscription.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("{id:int}/resume")]
    public async Task<IActionResult> ResumeSubscription(int id)
    {
        await _mediator.Send(new ResumeSubscriptionCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Settles tle subscription.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>SubscriptionSettlementDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("{id:int}/settle")]
    public async Task<ActionResult<SubscriptionSettlementDto>> SettleSubscription(int id)
    {
        var settlement = await _mediator.Send(new SettleSubscriptionCommand(id));
        return Ok(settlement);
    }

    /// <summary>
    /// Cancels subscription.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> CancelSubscription(int id)
    {
        await _mediator.Send(new CancelSubscriptionCommand(id));
        return NoContent();
    }

    // ==================
    // Delivery Actions
    // ==================

    /// <summary>
    /// Skips delivery.
    /// </summary>
    /// <param name="deliveryId">The delivery identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Creates or processes pay invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice identifier</param>
    /// <param name="request">The request body</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Creates a new one time delivery.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>OneTimeDeliveryDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("one-time-delivery")]
    public async Task<ActionResult<OneTimeDeliveryDto>> CreateOneTimeDelivery(
        [FromBody] CreateOneTimeDeliveryCommand command)
    {
        var delivery = await _mediator.Send(command);
        return Created("", delivery);
    }

    /// <summary>
    /// Converts to recurring.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>SubscriptionDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

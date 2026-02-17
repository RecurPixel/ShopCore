using ShopCore.Application.Payments.Commands.ConfirmPayment;
using ShopCore.Application.Payments.Commands.CreateInvoicePaymentIntent;
using ShopCore.Application.Payments.Commands.CreateOrderPaymentIntent;
using ShopCore.Application.Payments.Commands.HandlePaymentWebhook;
using ShopCore.Application.Payments.Commands.InitiateRefund;
using ShopCore.Application.Payments.Commands.RecordCODPayment;
using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ----------------
    // User-facing APIs
    // ----------------

    /// <summary>
    /// Creates a new order payment intent.
    /// </summary>
    /// <param name="orderId">The order identifier</param>
    /// <returns>PaymentIntentDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize]
    [HttpPost("orders/{orderId:int}/create-intent")]
    public async Task<ActionResult<PaymentIntentDto>> CreateOrderPaymentIntent(int orderId)
    {
        var intent = await _mediator.Send(new CreateOrderPaymentIntentCommand(orderId));
        return Ok(intent);
    }

    /// <summary>
    /// Creates a new invoice payment intent.
    /// </summary>
    /// <param name="invoiceId">The invoice identifier</param>
    /// <returns>PaymentIntentDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize]
    [HttpPost("invoices/{invoiceId:int}/create-intent")]
    public async Task<ActionResult<PaymentIntentDto>> CreateInvoicePaymentIntent(int invoiceId)
    {
        var intent = await _mediator.Send(new CreateInvoicePaymentIntentCommand(invoiceId));
        return Ok(intent);
    }

    /// <summary>
    /// Confirms payment.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>PaymentConfirmationDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize]
    [HttpPost("confirm")]
    public async Task<ActionResult<PaymentConfirmationDto>> ConfirmPayment(
        [FromBody] ConfirmPaymentCommand command)
    {
        var confirmation = await _mediator.Send(command);
        return Ok(confirmation);
    }

    /// <summary>
    /// Initiates refund.
    /// </summary>
    /// <param name="orderId">The order identifier</param>
    /// <param name="request">The request body</param>
    /// <returns>RefundDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize]
    [HttpPost("orders/{orderId:int}/refund")]
    public async Task<ActionResult<RefundDto>> InitiateRefund(
        int orderId,
        [FromBody] InitiateRefundRequest request)
    {
        var refund = await _mediator.Send(new InitiateRefundCommand(orderId, request.Amount, request.Reason));
        return Ok(refund);
    }

    /// <summary>
    /// Records cod payment.
    /// </summary>
    /// <param name="orderId">The order identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize]
    [HttpPost("orders/{orderId:int}/record-cod")]
    public async Task<IActionResult> RecordCODPayment(int orderId)
    {
        await _mediator.Send(new RecordCODPaymentCommand(orderId));
        return NoContent();
    }

    // -------------------
    // Webhook (No auth!)
    // -------------------

    /// <summary>
    /// Creates or processes handle webhook.
    /// </summary>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook(
        [FromHeader(Name = "X-Razorpay-Signature")] string signature)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();

        await _mediator.Send(new HandlePaymentWebhookCommand(payload, signature));
        return Ok();
    }
}

public record InitiateRefundRequest(decimal Amount, string? Reason = null);

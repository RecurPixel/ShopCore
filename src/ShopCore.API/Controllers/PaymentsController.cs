using ShopCore.Application.Payments.Commands.ConfirmPayment;
using ShopCore.Application.Payments.Commands.CreateInvoicePaymentIntent;
using ShopCore.Application.Payments.Commands.CreateOrderPaymentIntent;
using ShopCore.Application.Payments.Commands.HandlePaymentWebhook;
using ShopCore.Application.Payments.Commands.InitiateRefund;
using ShopCore.Application.Payments.Commands.RecordCODPayment;
using ShopCore.Application.Payments.DTOs;
using ShopCore.Application.Payments.Queries.GetAvailablePaymentOptions;
using ShopCore.Domain.Enums;

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
    // Payment Options
    // ----------------

    /// <summary>
    /// Gets available payment options/gateways.
    /// </summary>
    /// <returns>List of available payment options</returns>
    /// <response code="200">Returns available payment options</response>
    [AllowAnonymous]
    [HttpGet("options")]
    public async Task<ActionResult<IReadOnlyCollection<PaymentOptionDto>>> GetPaymentOptions()
    {
        var options = await _mediator.Send(new GetAvailablePaymentOptionsQuery());
        return Ok(options);
    }

    // ----------------
    // User-facing APIs
    // ----------------

    /// <summary>
    /// Creates a new order payment intent.
    /// </summary>
    /// <param name="orderId">The order identifier</param>
    /// <param name="gateway">Optional payment gateway to use (defaults to configured default)</param>
    /// <returns>PaymentIntentDto with gateway-specific client data</returns>
    /// <response code="200">Payment intent created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize]
    [HttpPost("orders/{orderId:int}/create-intent")]
    public async Task<ActionResult<PaymentIntentDto>> CreateOrderPaymentIntent(
        int orderId,
        [FromQuery] PaymentGateway? gateway = null)
    {
        var intent = await _mediator.Send(new CreateOrderPaymentIntentCommand(orderId, gateway));
        return Ok(intent);
    }

    /// <summary>
    /// Creates a new invoice payment intent.
    /// </summary>
    /// <param name="invoiceId">The invoice identifier</param>
    /// <param name="gateway">Optional payment gateway to use (defaults to configured default)</param>
    /// <returns>PaymentIntentDto with gateway-specific client data</returns>
    /// <response code="200">Payment intent created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize]
    [HttpPost("invoices/{invoiceId:int}/create-intent")]
    public async Task<ActionResult<PaymentIntentDto>> CreateInvoicePaymentIntent(
        int invoiceId,
        [FromQuery] PaymentGateway? gateway = null)
    {
        var intent = await _mediator.Send(new CreateInvoicePaymentIntentCommand(invoiceId, gateway));
        return Ok(intent);
    }

    /// <summary>
    /// Confirms payment after checkout completion.
    /// </summary>
    /// <param name="request">The payment confirmation request</param>
    /// <returns>PaymentConfirmationDto</returns>
    /// <response code="200">Payment confirmed successfully</response>
    /// <response code="400">Invalid request parameters or verification failed</response>
    /// <response code="401">Authentication required</response>
    [Authorize]
    [HttpPost("confirm")]
    public async Task<ActionResult<PaymentConfirmationDto>> ConfirmPayment(
        [FromBody] ConfirmPaymentRequest request)
    {
        var command = new ConfirmPaymentCommand(
            request.Gateway,
            request.GatewayOrderId,
            request.GatewayPaymentId,
            request.Signature,
            request.AdditionalData);

        var confirmation = await _mediator.Send(command);
        return Ok(confirmation);
    }

    /// <summary>
    /// Initiates refund for an order.
    /// </summary>
    /// <param name="orderId">The order identifier</param>
    /// <param name="request">The refund request</param>
    /// <returns>RefundDto</returns>
    /// <response code="200">Refund initiated successfully</response>
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
    /// Records COD payment collected at delivery.
    /// </summary>
    /// <param name="orderId">The order identifier</param>
    /// <returns>Status code indicating success</returns>
    /// <response code="204">COD payment recorded successfully</response>
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
    // Webhooks (No auth!)
    // -------------------

    /// <summary>
    /// Handles payment webhook from Razorpay.
    /// </summary>
    /// <returns>Status code indicating success</returns>
    /// <response code="200">Webhook processed successfully</response>
    /// <response code="400">Invalid webhook payload or signature</response>
    [AllowAnonymous]
    [HttpPost("webhook/razorpay")]
    public async Task<IActionResult> HandleRazorpayWebhook(
        [FromHeader(Name = "X-Razorpay-Signature")] string? signature)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();

        await _mediator.Send(new HandlePaymentWebhookCommand(
            PaymentGateway.Razorpay,
            payload,
            signature));

        return Ok();
    }

    /// <summary>
    /// Handles payment webhook from Stripe.
    /// </summary>
    /// <returns>Status code indicating success</returns>
    /// <response code="200">Webhook processed successfully</response>
    /// <response code="400">Invalid webhook payload or signature</response>
    [AllowAnonymous]
    [HttpPost("webhook/stripe")]
    public async Task<IActionResult> HandleStripeWebhook(
        [FromHeader(Name = "Stripe-Signature")] string? signature)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();

        await _mediator.Send(new HandlePaymentWebhookCommand(
            PaymentGateway.Stripe,
            payload,
            signature));

        return Ok();
    }

    /// <summary>
    /// Handles payment webhook from any gateway (generic endpoint).
    /// </summary>
    /// <param name="gateway">The payment gateway</param>
    /// <returns>Status code indicating success</returns>
    /// <response code="200">Webhook processed successfully</response>
    /// <response code="400">Invalid webhook payload or signature</response>
    [AllowAnonymous]
    [HttpPost("webhook/{gateway}")]
    public async Task<IActionResult> HandleGenericWebhook(PaymentGateway gateway)
    {
        using var reader = new StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();

        // Collect all headers for signature verification
        var headers = Request.Headers.ToDictionary(
            h => h.Key,
            h => h.Value.ToString());

        // Try to get signature from common header names
        var signature = headers.GetValueOrDefault("X-Razorpay-Signature")
                     ?? headers.GetValueOrDefault("Stripe-Signature")
                     ?? headers.GetValueOrDefault("X-Webhook-Signature");

        await _mediator.Send(new HandlePaymentWebhookCommand(
            gateway,
            payload,
            signature,
            headers));

        return Ok();
    }
}

/// <summary>
/// Request body for confirming a payment
/// </summary>
public record ConfirmPaymentRequest(
    PaymentGateway Gateway,
    string GatewayOrderId,
    string GatewayPaymentId,
    string? Signature = null,
    IDictionary<string, string>? AdditionalData = null);

/// <summary>
/// Request body for initiating a refund
/// </summary>
public record InitiateRefundRequest(decimal Amount, string? Reason = null);

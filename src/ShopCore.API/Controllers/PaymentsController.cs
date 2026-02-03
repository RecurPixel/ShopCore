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

    // POST /api/v1/payments/orders/{orderId}/create-intent
    [Authorize]
    [HttpPost("orders/{orderId:int}/create-intent")]
    public async Task<ActionResult<PaymentIntentDto>> CreateOrderPaymentIntent(int orderId)
    {
        var intent = await _mediator.Send(new CreateOrderPaymentIntentCommand(orderId));
        return Ok(intent);
    }

    // POST /api/v1/payments/invoices/{invoiceId}/create-intent
    [Authorize]
    [HttpPost("invoices/{invoiceId:int}/create-intent")]
    public async Task<ActionResult<PaymentIntentDto>> CreateInvoicePaymentIntent(int invoiceId)
    {
        var intent = await _mediator.Send(new CreateInvoicePaymentIntentCommand(invoiceId));
        return Ok(intent);
    }

    // POST /api/v1/payments/confirm
    [Authorize]
    [HttpPost("confirm")]
    public async Task<ActionResult<PaymentConfirmationDto>> ConfirmPayment(
        [FromBody] ConfirmPaymentCommand command)
    {
        var confirmation = await _mediator.Send(command);
        return Ok(confirmation);
    }

    // POST /api/v1/payments/orders/{orderId}/refund
    [Authorize]
    [HttpPost("orders/{orderId:int}/refund")]
    public async Task<ActionResult<RefundDto>> InitiateRefund(
        int orderId,
        [FromBody] InitiateRefundRequest request)
    {
        var refund = await _mediator.Send(new InitiateRefundCommand(orderId, request.Amount, request.Reason));
        return Ok(refund);
    }

    // POST /api/v1/payments/orders/{orderId}/record-cod
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

    // POST /api/v1/payments/webhook
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

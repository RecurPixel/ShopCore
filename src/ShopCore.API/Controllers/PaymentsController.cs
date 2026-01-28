using ShopCore.Application.Payments.Commands.ConfirmPayment;
using ShopCore.Application.Payments.Commands.CreateOrderPaymentIntent;
using ShopCore.Application.Payments.Commands.CreateInvoicePaymentIntent;
using ShopCore.Application.Payments.Commands.HandlePaymentWebhook;
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

    // -------------------
    // Webhook (No auth!)
    // -------------------

    // POST /api/v1/payments/webhook
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        await _mediator.Send(new HandlePaymentWebhookCommand());
        return Ok();
    }
}

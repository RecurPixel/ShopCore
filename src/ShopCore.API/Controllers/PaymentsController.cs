using ShopCore.Application.Common.Models;
using ShopCore.Application.Payments.Commands.ConfirmPayment;
using ShopCore.Application.Payments.Commands.CreatePaymentIntent;
using ShopCore.Application.Payments.Commands.HandlePaymentWebhook;
using ShopCore.Application.Payments.DTOs;
using ShopCore.Application.Payments.Queries.GetPaymentHistory;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
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

    // POST /api/v1/payments/create-intent
    [Authorize]
    [HttpPost("create-intent")]
    public async Task<ActionResult<PaymentIntentDto>> CreatePaymentIntent(
        [FromBody] CreatePaymentIntentCommand command)
    {
        var intent = await _mediator.Send(command);
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

    // GET /api/v1/payments/history
    [Authorize]
    [HttpGet("history")]
    public async Task<ActionResult<PaginatedList<PaymentHistoryDto>>> GetPaymentHistory(
        [FromQuery] GetPaymentHistoryQuery query)
    {
        var history = await _mediator.Send(query);
        return Ok(history);
    }

    // -------------------
    // Webhook (No auth!)
    // -------------------

    // POST /api/v1/payments/webhook
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        // Raw body should be read inside the command/handler
        await _mediator.Send(new HandlePaymentWebhookCommand());
        return Ok();
    }
}

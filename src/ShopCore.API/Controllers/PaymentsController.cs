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
    public async Task<IActionResult> CreatePaymentIntent(
        [FromBody] CreatePaymentIntentCommand command
    )
    {
        var intent = await _mediator.Send(command);
        return Ok(intent);
    }

    // POST /api/v1/payments/confirm
    [Authorize]
    [HttpPost("confirm")]
    public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // GET /api/v1/payments/history
    [Authorize]
    [HttpGet("history")]
    public async Task<IActionResult> GetPaymentHistory([FromQuery] GetPaymentHistoryQuery query)
    {
        var payments = await _mediator.Send(query);
        return Ok(payments);
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

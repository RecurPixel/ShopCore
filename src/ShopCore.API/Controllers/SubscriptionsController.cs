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
    [HttpPost]
    public async Task<IActionResult> CreateSubscription(
        [FromBody] CreateSubscriptionCommand command
    )
    {
        var subscription = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetSubscriptionById),
            new { id = subscription.Id },
            subscription
        );
    }

    // GET /api/v1/subscriptions
    [HttpGet]
    public async Task<IActionResult> GetMySubscriptions([FromQuery] GetMySubscriptionsQuery query)
    {
        var subscriptions = await _mediator.Send(query);
        return Ok(subscriptions);
    }

    // GET /api/v1/subscriptions/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubscriptionById(Guid id)
    {
        var subscription = await _mediator.Send(new GetSubscriptionByIdQuery(id));

        if (subscription == null)
            return NotFound();

        return Ok(subscription);
    }

    // PATCH /api/v1/subscriptions/{id}
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateSubscription(
        Guid id,
        [FromBody] UpdateSubscriptionCommand command
    )
    {
        command.SubscriptionId = id;

        var subscription = await _mediator.Send(command);
        return Ok(subscription);
    }

    // PATCH /api/v1/subscriptions/{id}/pause
    [HttpPatch("{id}/pause")]
    public async Task<IActionResult> PauseSubscription(Guid id)
    {
        await _mediator.Send(new PauseSubscriptionCommand(id));
        return NoContent();
    }

    // PATCH /api/v1/subscriptions/{id}/resume
    [HttpPatch("{id}/resume")]
    public async Task<IActionResult> ResumeSubscription(Guid id)
    {
        await _mediator.Send(new ResumeSubscriptionCommand(id));
        return NoContent();
    }

    // POST /api/v1/subscriptions/{id}/settle
    [HttpPost("{id}/settle")]
    public async Task<IActionResult> SettleSubscription(Guid id)
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
    public async Task<IActionResult> GetVendorSubscriptions(
        [FromQuery] GetVendorSubscriptionsQuery query
    )
    {
        var subscriptions = await _mediator.Send(query);
        return Ok(subscriptions);
    }
}

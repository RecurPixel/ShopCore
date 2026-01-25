namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DeliveriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DeliveriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ----------------
    // User endpoints
    // ----------------

    // GET /api/v1/deliveries/subscriptions/{id}/deliveries
    [Authorize]
    [HttpGet("subscriptions/{id}/deliveries")]
    public async Task<IActionResult> GetSubscriptionDeliveries(Guid id)
    {
        var deliveries = await _mediator.Send(new GetSubscriptionDeliveriesQuery(id));

        return Ok(deliveries);
    }

    // GET /api/v1/deliveries/{id}
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDeliveryById(Guid id)
    {
        var delivery = await _mediator.Send(new GetDeliveryByIdQuery(id));

        if (delivery == null)
            return NotFound();

        return Ok(delivery);
    }

    // POST /api/v1/deliveries/{id}/skip
    [Authorize]
    [HttpPost("{id}/skip")]
    public async Task<IActionResult> SkipDelivery(Guid id)
    {
        await _mediator.Send(new SkipDeliveryCommand(id));
        return NoContent();
    }

    // ----------------
    // Vendor endpoints
    // ----------------

    // PATCH /api/v1/deliveries/{id}/complete
    [Authorize(Roles = "Vendor")]
    [HttpPatch("{id}/complete")]
    public async Task<IActionResult> CompleteDelivery(Guid id)
    {
        await _mediator.Send(new CompleteDeliveryCommand(id));
        return NoContent();
    }

    // PATCH /api/v1/deliveries/{id}/failed
    [Authorize(Roles = "Vendor")]
    [HttpPatch("{id}/failed")]
    public async Task<IActionResult> FailDelivery(Guid id)
    {
        await _mediator.Send(new FailDeliveryCommand(id));
        return NoContent();
    }
}

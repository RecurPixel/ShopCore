using ShopCore.Application.Common.Models;
using ShopCore.Application.Deliveries.Commands.CompleteDelivery;
using ShopCore.Application.Deliveries.Commands.FailDelivery;
using ShopCore.Application.Deliveries.Commands.SkipDelivery;
using ShopCore.Application.Deliveries.DTOs;
using ShopCore.Application.Deliveries.Queries.GetDeliveryById;
using ShopCore.Application.Deliveries.Queries.GetSubscriptionDeliveries;

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
    public async Task<ActionResult<PaginatedList<DeliveryDto>>> GetSubscriptionDeliveries(
        [FromRoute] int subscriptionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var deliveries = await _mediator.Send(new GetSubscriptionDeliveriesQuery(id));

        return Ok(deliveries);
    }

    // GET /api/v1/deliveries/{id}
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<DeliveryDto>> GetDeliveryById(
        [FromRoute] int id)
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

    // GET /api/v1/deliveries/upcoming
    [Authorize(Roles = "Vendor,Driver")]
    [HttpGet("upcoming")]
    public async Task<ActionResult<List<DeliveryDto>>> GetUpcomingDeliveries(
        [FromQuery] DateTime? date = null)
    {
        var deliveries = await _mediator.Send(
            new GetUpcomingDeliveriesQuery(date)
        );
        return Ok(deliveries);
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

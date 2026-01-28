using ShopCore.Application.Orders.Commands.CreateOrder;
using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Api.Controllers;

/// <summary>
/// Order creation endpoint.
/// Order viewing and management is at /api/v1/users/me/orders
/// Vendor order management is at /api/v1/vendors/me/orders
/// </summary>
[ApiController]
[Authorize]
[Route("api/v1/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/v1/orders
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(
        [FromBody] CreateOrderCommand command)
    {
        var order = await _mediator.Send(command);
        return CreatedAtRoute(
            routeName: null,
            routeValues: new { id = order.Id },
            value: order);
    }
}

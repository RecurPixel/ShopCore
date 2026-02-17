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

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>OrderDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

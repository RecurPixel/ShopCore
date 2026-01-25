using ShopCore.Application.Orders.Commands.CancelOrder;
using ShopCore.Application.Orders.Commands.CreateOrder;
using ShopCore.Application.Orders.Queries.GetMyOrders;
using ShopCore.Application.Orders.Queries.GetOrderById;
using ShopCore.Application.Orders.Queries.GetOrderInvoice;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // POST /api/v1/orders
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var order = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
    }

    // GET /api/v1/orders
    [HttpGet]
    public async Task<IActionResult> GetMyOrders([FromQuery] GetMyOrdersQuery query)
    {
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    // GET /api/v1/orders/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));

        if (order == null)
            return NotFound();

        return Ok(order);
    }

    // POST /api/v1/orders/{id}/cancel
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        await _mediator.Send(new CancelOrderCommand(id));
        return NoContent();
    }

    // GET /api/v1/orders/{id}/invoice
    [HttpGet("{id}/invoice")]
    public async Task<IActionResult> GetInvoice(int id)
    {
        var invoice = await _mediator.Send(new GetOrderInvoiceQuery(id));

        if (invoice == null)
            return NotFound();

        return Ok(invoice);
    }
}

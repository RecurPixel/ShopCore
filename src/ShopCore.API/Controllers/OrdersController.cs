using ShopCore.Application.Common.Models;
using ShopCore.Application.Orders.Commands.CancelOrder;
using ShopCore.Application.Orders.Commands.CreateOrder;
using ShopCore.Application.Orders.DTOs;
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
    public async Task<ActionResult<OrderDto>> CreateOrder(
        [FromBody] CreateOrderCommand command)
    {
        var order = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetOrderById),
            new { id = order.Id },
            order);
    }

    // GET /api/v1/orders
    [HttpGet]
    public async Task<ActionResult<PaginatedList<OrderDto>>> GetMyOrders(
        [FromQuery] GetMyOrdersQuery query)
    {
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    // GET /api/v1/orders/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDetailDto>> GetOrderById(int id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));

        if (order is null)
            return NotFound();

        return Ok(order);
    }

    // POST /api/v1/orders/{id}/cancel
    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> CancelOrder(
        int id,
        [FromBody] CancelOrderCommand command)
    {
        var finalCommand = command with { OrderId = id };

        await _mediator.Send(finalCommand);
        return NoContent();
    }

    // GET /api/v1/orders/{id}/invoice
    [HttpGet("{id:int}/invoice")]
    public async Task<IActionResult> GetOrderInvoice(int id)
    {
        var invoiceBytes = await _mediator.Send(
            new GetOrderInvoiceQuery(id));

        if (invoiceBytes.FileContent is null || invoiceBytes.Length == 0)
            return NotFound();

        return File(
            invoiceBytes.FileContent,
            "application/pdf",
            $"invoice-order-{id}.pdf");
    }
}

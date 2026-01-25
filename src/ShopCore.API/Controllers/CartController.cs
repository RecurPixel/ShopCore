namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/v1/cart
    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var cart = await _mediator.Send(new GetCartQuery());
        return Ok(cart);
    }

    // POST /api/v1/cart/items
    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemCommand command)
    {
        var cart = await _mediator.Send(command);
        return Ok(cart);
    }

    // PUT /api/v1/cart/items/{id}
    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateCartItemCommand command)
    {
        command.CartItemId = id;

        var cart = await _mediator.Send(command);
        return Ok(cart);
    }

    // DELETE /api/v1/cart/items/{id}
    [HttpDelete("items/{id}")]
    public async Task<IActionResult> RemoveItem(Guid id)
    {
        await _mediator.Send(new RemoveCartItemCommand(id));
        return NoContent();
    }

    // DELETE /api/v1/cart/clear
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        await _mediator.Send(new ClearCartCommand());
        return NoContent();
    }

    // POST /api/v1/cart/validate
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateCart()
    {
        var result = await _mediator.Send(new ValidateCartCommand());
        return Ok(result);
    }
}

using ShopCore.Application.Cart.Commands.AddCartItem;
using ShopCore.Application.Cart.Commands.ClearCart;
using ShopCore.Application.Cart.Commands.RemoveCartItem;
using ShopCore.Application.Cart.Commands.UpdateCartItem;
using ShopCore.Application.Cart.Commands.ValidateCart;
using ShopCore.Application.Cart.DTOs;
using ShopCore.Application.Cart.Queries.GetCart;

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
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var cart = await _mediator.Send(new GetCartQuery());
        return Ok(cart);
    }

    // POST /api/v1/cart/items
    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddToCart(
      [FromBody] AddCartItemCommand command)
    {
        var cart = await _mediator.Send(command);
        return Ok(cart);
    }

    // PUT /api/v1/cart/items/{id}
    [HttpPut("items/{id:int}")]
    public async Task<ActionResult<CartDto>> UpdateCartItem(
        int id,
        [FromBody] UpdateCartItemCommand command)
    {
        var finalCommand = command with { CartItemId = id };

        var cart = await _mediator.Send(finalCommand);
        return Ok(cart);
    }

    // DELETE /api/v1/cart/items/{id}
    [HttpDelete("items/{id:int}")]
    public async Task<ActionResult<CartDto>> RemoveFromCart(int id)
    {
        var cart = await _mediator.Send(
            new RemoveCartItemCommand(id));

        return Ok(cart);
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
    public async Task<ActionResult<CartValidationResultDto>> ValidateCart()
    {
        var result = await _mediator.Send(new ValidateCartCommand());
        return Ok(result);
    }
}

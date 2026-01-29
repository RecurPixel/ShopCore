using ShopCore.Application.Cart.Commands.AddCartItem;
using ShopCore.Application.Cart.Commands.ApplyCoupon;
using ShopCore.Application.Cart.Commands.ClearCart;
using ShopCore.Application.Cart.Commands.RemoveCartItem;
using ShopCore.Application.Cart.Commands.RemoveCoupon;
using ShopCore.Application.Cart.Commands.UpdateCartItem;
using ShopCore.Application.Cart.Commands.ValidateCart;
using ShopCore.Application.Cart.DTOs;
using ShopCore.Application.Cart.Queries.GetCart;

namespace ShopCore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/cart")]
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

    // PUT /api/v1/cart/items/{itemId}
    [HttpPut("items/{itemId:int}")]
    public async Task<ActionResult<CartDto>> UpdateCartItem(
        int itemId,
        [FromBody] UpdateCartItemCommand command)
    {
        var finalCommand = command with { CartItemId = itemId };
        var cart = await _mediator.Send(finalCommand);
        return Ok(cart);
    }

    // DELETE /api/v1/cart/items/{itemId}
    [HttpDelete("items/{itemId:int}")]
    public async Task<ActionResult<CartDto>> RemoveFromCart(int itemId)
    {
        var cart = await _mediator.Send(new RemoveCartItemCommand(itemId));
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

    // POST /api/v1/cart/apply-coupon
    [HttpPost("apply-coupon")]
    public async Task<ActionResult<CartDto>> ApplyCoupon(
        [FromBody] ApplyCouponCommand command)
    {
        var cart = await _mediator.Send(command);
        return Ok(cart);
    }

    // DELETE /api/v1/cart/coupon
    [HttpDelete("coupon")]
    public async Task<ActionResult<CartDto>> RemoveCoupon()
    {
        var cart = await _mediator.Send(new RemoveCouponCommand());
        return Ok(cart);
    }
}

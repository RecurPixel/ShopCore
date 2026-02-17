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

    /// <summary>
    /// Retrieves cart.
    /// </summary>
    /// <returns>CartDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    /// <response code="404">Resource not found</response>
    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart()
    {
        var cart = await _mediator.Send(new GetCartQuery());
        return Ok(cart);
    }

    /// <summary>
    /// Adds to cart.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>CartDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddToCart(
        [FromBody] AddCartItemCommand command)
    {
        var cart = await _mediator.Send(command);
        return Ok(cart);
    }

    /// <summary>
    /// Updates cart item.
    /// </summary>
    /// <param name="itemId">The item identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>CartDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPut("items/{itemId:int}")]
    public async Task<ActionResult<CartDto>> UpdateCartItem(
        int itemId,
        [FromBody] UpdateCartItemCommand command)
    {
        var finalCommand = command with { CartItemId = itemId };
        var cart = await _mediator.Send(finalCommand);
        return Ok(cart);
    }

    /// <summary>
    /// Removes from cart.
    /// </summary>
    /// <param name="itemId">The item identifier</param>
    /// <returns>CartDto</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpDelete("items/{itemId:int}")]
    public async Task<ActionResult<CartDto>> RemoveFromCart(int itemId)
    {
        var cart = await _mediator.Send(new RemoveCartItemCommand(itemId));
        return Ok(cart);
    }

    /// <summary>
    /// Clears cart.
    /// </summary>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        await _mediator.Send(new ClearCartCommand());
        return NoContent();
    }

    /// <summary>
    /// Validates cart.
    /// </summary>
    /// <returns>CartValidationResultDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("validate")]
    public async Task<ActionResult<CartValidationResultDto>> ValidateCart()
    {
        var result = await _mediator.Send(new ValidateCartCommand());
        return Ok(result);
    }

    /// <summary>
    /// Applies coupon.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>CartDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpPost("apply-coupon")]
    public async Task<ActionResult<CartDto>> ApplyCoupon(
        [FromBody] ApplyCouponCommand command)
    {
        var cart = await _mediator.Send(command);
        return Ok(cart);
    }

    /// <summary>
    /// Removes coupon.
    /// </summary>
    /// <returns>CartDto</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [HttpDelete("remove-coupon")]
    public async Task<ActionResult<CartDto>> RemoveCoupon()
    {
        var cart = await _mediator.Send(new RemoveCouponCommand());
        return Ok(cart);
    }
}

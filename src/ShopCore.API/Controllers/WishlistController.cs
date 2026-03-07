using ShopCore.Application.Cart.DTOs;
using ShopCore.Application.Wishlist.Commands.AddToWishlist;
using ShopCore.Application.Wishlist.Commands.MoveToCart;
using ShopCore.Application.Wishlist.Commands.RemoveFromWishlist;
using ShopCore.Application.Wishlist.DTOs;
using ShopCore.Application.Wishlist.Queries.GetWishlist;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/wishlist")]
public class WishlistController : ControllerBase
{
    private readonly IMediator _mediator;

    public WishlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves wishlist.
    /// </summary>
    /// <returns>WishlistDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [HttpGet]
    public async Task<ActionResult<WishlistDto>> GetWishlist()
    {
        var wishlist = await _mediator.Send(new GetWishlistQuery());
        return Ok(wishlist);
    }

    /// <summary>
    /// Adds to wishlist.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPost]
    public async Task<IActionResult> AddToWishlist(
        [FromBody] AddToWishlistCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Removes from wishlist.
    /// </summary>
    /// <param name="productId">The product identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        await _mediator.Send(
            new RemoveFromWishlistCommand(productId));

        return NoContent();
    }

    /// <summary>
    /// Moves to cart.
    /// </summary>
    /// <param name="productId">The product identifier</param>
    /// <returns>CartDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPost("{productId:int}/move-to-cart")]
    public async Task<ActionResult<CartDto>> MoveToCart(int productId)
    {
        var cart = await _mediator.Send(
            new MoveToCartCommand(productId));

        return Ok(cart);
    }
}

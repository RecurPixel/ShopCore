using ShopCore.Application.Cart.DTOs;
using ShopCore.Application.Wishlist.Commands.AddToWishlist;
using ShopCore.Application.Wishlist.Commands.RemoveFromWishlist;
using ShopCore.Application.Wishlist.DTOs;
using ShopCore.Application.Wishlist.Queries.GetWishlist;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IMediator _mediator;

    public WishlistController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/v1/wishlist
    [HttpGet]
    public async Task<ActionResult<WishlistDto>> GetWishlist()
    {
        var wishlist = await _mediator.Send(new GetWishlistQuery());
        return Ok(wishlist);
    }

    // POST /api/v1/wishlist
    [HttpPost]
    public async Task<IActionResult> AddToWishlist(
        [FromBody] AddToWishlistCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE /api/v1/wishlist/{productId}
    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> RemoveFromWishlist(int productId)
    {
        await _mediator.Send(
            new RemoveFromWishlistCommand(productId));

        return NoContent();
    }

    // POST /api/v1/wishlist/{productId}/move-to-cart
    [HttpPost("{productId:int}/move-to-cart")]
    public async Task<ActionResult<CartDto>> MoveToCart(int productId)
    {
        var cart = await _mediator.Send(
            new MoveToCartCommand(productId));

        return Ok(cart);
    }
}

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
    public async Task<IActionResult> GetWishlist()
    {
        var wishlist = await _mediator.Send(new GetWishlistQuery());
        return Ok(wishlist);
    }

    // POST /api/v1/wishlist
    [HttpPost]
    public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }

    // DELETE /api/v1/wishlist/{productId}
    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromWishlist(Guid productId)
    {
        await _mediator.Send(new RemoveFromWishlistCommand(productId));

        return NoContent();
    }
}

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CouponsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CouponsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ----------------
    // User-facing APIs
    // ----------------

    // GET /api/v1/coupons/active
    [Authorize]
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveCoupons()
    {
        var coupons = await _mediator.Send(new GetActiveCouponsQuery());
        return Ok(coupons);
    }

    // POST /api/v1/coupons/validate
    [Authorize]
    [HttpPost("validate")]
    public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    // -------------
    // Admin actions
    // -------------

    // GET /api/v1/coupons
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllCoupons()
    {
        var coupons = await _mediator.Send(new GetAllCouponsQuery());
        return Ok(coupons);
    }

    // POST /api/v1/coupons
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponCommand command)
    {
        var coupon = await _mediator.Send(command);
        return Ok(coupon);
    }

    // PATCH /api/v1/coupons/{id}/deactivate
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> DeactivateCoupon(Guid id)
    {
        await _mediator.Send(new DeactivateCouponCommand(id));
        return NoContent();
    }
}

using ShopCore.Application.Coupons.Commands.CreateCoupon;
using ShopCore.Application.Coupons.Commands.DeactivateCoupon;
using ShopCore.Application.Coupons.Commands.DeleteCoupon;
using ShopCore.Application.Coupons.Commands.UpdateCoupon;
using ShopCore.Application.Coupons.Commands.ValidateCoupon;
using ShopCore.Application.Coupons.DTOs;
using ShopCore.Application.Coupons.Queries.GetActiveCoupons;
using ShopCore.Application.Coupons.Queries.GetAllCoupons;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/coupons")]
public class CouponsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CouponsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ----------------
    // Public endpoints
    // ----------------

    // GET /api/v1/coupons/active
    [HttpGet("active")]
    public async Task<ActionResult<List<CouponDto>>> GetActiveCoupons()
    {
        var coupons = await _mediator.Send(new GetActiveCouponsQuery());
        return Ok(coupons);
    }

    // POST /api/v1/coupons/validate
    [Authorize]
    [HttpPost("validate")]
    public async Task<ActionResult<CouponValidationResultDto>> ValidateCoupon(
        [FromBody] ValidateCouponCommand command)
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
    public async Task<ActionResult<List<CouponDto>>> GetAllCoupons()
    {
        var coupons = await _mediator.Send(new GetAllCouponsQuery());
        return Ok(coupons);
    }

    // POST /api/v1/coupons
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CouponDto>> CreateCoupon(
        [FromBody] CreateCouponCommand command)
    {
        var coupon = await _mediator.Send(command);
        return Ok(coupon);
    }

    // PUT /api/v1/coupons/{id}
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CouponDto>> UpdateCoupon(int id, [FromBody] UpdateCouponRequest request)
    {
        var command = new UpdateCouponCommand(
            id,
            request.Code,
            request.DiscountType,
            request.DiscountValue,
            request.MinOrderAmount,
            request.MaxDiscountAmount,
            request.UsageLimit,
            request.StartDate,
            request.EndDate,
            request.IsActive);
        var coupon = await _mediator.Send(command);
        return Ok(coupon);
    }

    // PATCH /api/v1/coupons/{id}/deactivate
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateCoupon(int id)
    {
        await _mediator.Send(new DeactivateCouponCommand(id));
        return NoContent();
    }

    // DELETE /api/v1/coupons/{id}
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCoupon(int id)
    {
        await _mediator.Send(new DeleteCouponCommand(id));
        return NoContent();
    }
}

public record UpdateCouponRequest(
    string Code,
    string DiscountType,
    decimal DiscountValue,
    decimal? MinOrderAmount,
    decimal? MaxDiscountAmount,
    int? UsageLimit,
    DateTime? StartDate,
    DateTime? EndDate,
    bool IsActive);

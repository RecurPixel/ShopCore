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

    /// <summary>
    /// Retrieves active coupons.
    /// </summary>
    /// <returns>List&lt;CouponDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("active")]
    public async Task<ActionResult<List<CouponDto>>> GetActiveCoupons()
    {
        var coupons = await _mediator.Send(new GetActiveCouponsQuery());
        return Ok(coupons);
    }

    /// <summary>
    /// Validates coupon.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>CouponValidationResultDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

    /// <summary>
    /// Retrieves all coupons.
    /// </summary>
    /// <returns>List&lt;CouponDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<CouponDto>>> GetAllCoupons()
    {
        var coupons = await _mediator.Send(new GetAllCouponsQuery());
        return Ok(coupons);
    }

    /// <summary>
    /// Creates a new coupon.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>CouponDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<CouponDto>> CreateCoupon(
        [FromBody] CreateCouponCommand command)
    {
        var coupon = await _mediator.Send(command);
        return Ok(coupon);
    }

    /// <summary>
    /// Updates coupon.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="request">The request body</param>
    /// <returns>CouponDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<CouponDto>> UpdateCoupon(int id, [FromBody] UpdateCouponRequest request)
    {
        // IsActive is controlled via ActivateCouponCommand/DeactivateCouponCommand
        var command = new UpdateCouponCommand(
            id,
            request.Code,
            request.DiscountType,
            request.DiscountValue,
            request.MinOrderAmount,
            request.MaxDiscountAmount,
            request.UsageLimit,
            request.StartDate,
            request.EndDate);
        var coupon = await _mediator.Send(command);
        return Ok(coupon);
    }

    /// <summary>
    /// Partially updates deactivate coupon.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateCoupon(int id)
    {
        await _mediator.Send(new DeactivateCouponCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Deletes coupon.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
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

using ShopCore.Application.Common.Models;
using ShopCore.Application.Payouts.Commands.CalculateVendorPayout;
using ShopCore.Application.Payouts.Commands.ProcessVendorPayout;
using ShopCore.Application.Payouts.DTOs;
using ShopCore.Application.Payouts.Queries.GetPendingPayoutAmount;
using ShopCore.Application.Payouts.Queries.GetVendorPayouts;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/payouts")]
public class PayoutsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PayoutsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves vendor payouts.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;VendorPayoutDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [HttpGet]
    public async Task<ActionResult<PaginatedList<VendorPayoutDto>>> GetVendorPayouts(
        [FromQuery] GetVendorPayoutsQuery query)
    {
        var payouts = await _mediator.Send(query);
        return Ok(payouts);
    }

    /// <summary>
    /// Retrieves pending payout.
    /// </summary>
    /// <returns>VendorPayoutDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Vendor")]
    [HttpGet("pending")]
    public async Task<ActionResult<VendorPayoutDto>> GetPendingPayout()
    {
        var payout = await _mediator.Send(new GetPendingPayoutAmountQuery());
        return Ok(payout);
    }

    /// <summary>
    /// Creates or processes calculate payouts.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>List&lt;VendorPayoutDto&gt;</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("calculate")]
    public async Task<ActionResult<List<VendorPayoutDto>>> CalculatePayouts(
        [FromBody] CalculateVendorPayoutCommand command)
    {
        var payouts = await _mediator.Send(command);
        return Ok(payouts);
    }

    /// <summary>
    /// Processes payout.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="401">Authentication required</response>
    [Authorize(Roles = "Admin")]
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayout(
        [FromBody] ProcessVendorPayoutCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}

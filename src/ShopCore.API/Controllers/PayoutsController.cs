using ShopCore.Application.Common.Models;
using ShopCore.Application.Payouts.Commands.CalculateVendorPayout;
using ShopCore.Application.Payouts.Commands.ProcessVendorPayout;
using ShopCore.Application.Payouts.DTOs;
using ShopCore.Application.Payouts.Queries.GetVendorPayouts;

namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PayoutsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PayoutsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/v1/payouts
    [HttpGet]
    public async Task<ActionResult<PaginatedList<VendorPayoutDto>>> GetVendorPayouts(
        [FromQuery] GetVendorPayoutsQuery query)
    {
        var payouts = await _mediator.Send(query);
        return Ok(payouts);
    }

    // GET /api/v1/payouts/pending
    [Authorize(Roles = "Vendor")]
    [HttpGet("pending")]
    public async Task<ActionResult<VendorPayoutDto>> GetPendingPayout()
    {
        var payout = await _mediator.Send(new GetPendingPayoutQuery());
        return Ok(payout);
    }

    // POST /api/v1/payouts/calculate
    [Authorize(Roles = "Admin")]
    [HttpPost("calculate")]
    public async Task<ActionResult<List<VendorPayoutDto>>> CalculatePayouts(
        [FromBody] CalculateVendorPayoutCommand command)
    {
        var payouts = await _mediator.Send(command);
        return Ok(payouts);
    }

    // POST /api/v1/payouts/process
    [Authorize(Roles = "Admin")]
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayout(
        [FromBody] ProcessVendorPayoutCommand command)
    {
        await _mediator.Send(command);
        return NoContent();
    }
}

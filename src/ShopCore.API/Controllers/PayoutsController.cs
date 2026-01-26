using ShopCore.Application.Payouts.Commands.CalculateVendorPayout;
using ShopCore.Application.Payouts.Commands.ProcessVendorPayout;
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
    public async Task<IActionResult> GetPayouts([FromQuery] GetVendorPayoutsQuery query)
    {
        var payouts = await _mediator.Send(query);
        return Ok(payouts);
    }

    // POST /api/v1/payouts/calculate
    [HttpPost("calculate")]
    public async Task<IActionResult> CalculatePayout()
    {
        var payout = await _mediator.Send(new CalculateVendorPayoutCommand());

        return Ok(payout);
    }

    // POST /api/v1/payouts/process
    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayout()
    {
        await _mediator.Send(new ProcessVendorPayoutCommand());
        return NoContent();
    }
}

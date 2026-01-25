namespace ShopCore.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class VendorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public VendorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // --------------------
    // Vendor (Self-service)
    // --------------------

    // POST /api/v1/vendors/register
    [Authorize]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterVendor([FromBody] RegisterVendorCommand command)
    {
        var vendor = await _mediator.Send(command);
        return Ok(vendor);
    }

    // GET /api/v1/vendors/me
    [Authorize(Roles = "Vendor")]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyVendorProfile()
    {
        var vendor = await _mediator.Send(new GetMyVendorQuery());
        return Ok(vendor);
    }

    // PUT /api/v1/vendors/me
    [Authorize(Roles = "Vendor")]
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyVendorProfile([FromBody] UpdateMyVendorCommand command)
    {
        var vendor = await _mediator.Send(command);
        return Ok(vendor);
    }

    // GET /api/v1/vendors/me/stats
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/stats")]
    public async Task<IActionResult> GetMyVendorStats()
    {
        var stats = await _mediator.Send(new GetMyVendorStatsQuery());
        return Ok(stats);
    }

    // GET /api/v1/vendors/me/orders
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/orders")]
    public async Task<IActionResult> GetMyOrders()
    {
        var orders = await _mediator.Send(new GetMyVendorOrdersQuery());
        return Ok(orders);
    }

    // PATCH /api/v1/vendors/me/orders/{id}/status
    [Authorize(Roles = "Vendor")]
    [HttpPatch("me/orders/{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(
        Guid id,
        [FromBody] UpdateVendorOrderStatusCommand command
    )
    {
        command.OrderId = id;

        await _mediator.Send(command);
        return NoContent();
    }

    // -------------
    // Admin actions
    // -------------

    // GET /api/v1/vendors/pending
    [Authorize(Roles = "Admin")]
    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingVendors()
    {
        var vendors = await _mediator.Send(new GetPendingVendorsQuery());
        return Ok(vendors);
    }

    // PATCH /api/v1/vendors/{id}/approve
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/approve")]
    public async Task<IActionResult> ApproveVendor(Guid id)
    {
        await _mediator.Send(new ApproveVendorCommand(id));
        return NoContent();
    }

    // PATCH /api/v1/vendors/{id}/suspend
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}/suspend")]
    public async Task<IActionResult> SuspendVendor(Guid id)
    {
        await _mediator.Send(new SuspendVendorCommand(id));
        return NoContent();
    }
}

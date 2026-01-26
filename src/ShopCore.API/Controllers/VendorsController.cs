using ShopCore.Api.Files;
using ShopCore.Application.Common.Models;
using ShopCore.Application.Vendors.Commands.ApproveVendor;
using ShopCore.Application.Vendors.Commands.RegisterVendor;
using ShopCore.Application.Vendors.Commands.SuspendVendor;
using ShopCore.Application.Vendors.Commands.UpdateMyVendor;
using ShopCore.Application.Vendors.Commands.UpdateVendorOrderStatus;
using ShopCore.Application.Vendors.Commands.UploadVendorLogo;
using ShopCore.Application.Vendors.DTOs;
using ShopCore.Application.Vendors.Queries.GetMyVendor;
using ShopCore.Application.Vendors.Queries.GetMyVendorOrders;
using ShopCore.Application.Vendors.Queries.GetMyVendorStats;
using ShopCore.Application.Vendors.Queries.GetPendingVendors;

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
    public async Task<ActionResult<VendorProfileDto>> RegisterVendor(
        [FromBody] RegisterVendorCommand command)
    {
        var vendor = await _mediator.Send(command);
        return Ok(vendor);
    }

    // GET /api/v1/vendors/me
    [Authorize(Roles = "Vendor")]
    [HttpGet("me")]
    public async Task<ActionResult<VendorProfileDto>> GetMyVendorProfile()
    {
        var vendor = await _mediator.Send(new GetMyVendorQuery());
        return Ok(vendor);
    }

    // PUT /api/v1/vendors/me
    [Authorize(Roles = "Vendor")]
    [HttpPut("me")]
    public async Task<ActionResult<VendorProfileDto>> UpdateVendorProfile(
        [FromBody] UpdateMyVendorCommand command)
    {
        var vendor = await _mediator.Send(command);
        return Ok(vendor);
    }

    // POST /api/v1/vendors/me/logo
    [Authorize(Roles = "Vendor")]
    [HttpPost("me/logo")]
    public async Task<ActionResult<string>> UploadVendorLogo(
        IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("Logo file is required.");

        var command = new UploadVendorLogoCommand(
            new FormFileAdapter(file));

        var logoUrl = await _mediator.Send(command);
        return Ok(logoUrl);
    }

    // GET /api/v1/vendors/me/stats
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/stats")]
    public async Task<ActionResult<VendorStatsDto>> GetVendorStats()
    {
        var stats = await _mediator.Send(new GetMyVendorStatsQuery());
        return Ok(stats);
    }

    // GET /api/v1/vendors/me/orders
    [Authorize(Roles = "Vendor")]
    [HttpGet("me/orders")]
    public async Task<ActionResult<PaginatedList<VendorOrderDto>>> GetVendorOrders(
        [FromQuery] GetMyVendorOrdersQuery query)
    {
        var orders = await _mediator.Send(query);
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
    public async Task<ActionResult<List<VendorProfileDto>>> GetPendingVendors()
    {
        var vendors = await _mediator.Send(new GetPendingVendorsQuery());
        return Ok(vendors);
    }

    // PATCH /api/v1/vendors/{id}/approve
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/approve")]
    public async Task<IActionResult> ApproveVendor(int id)
    {
        await _mediator.Send(new ApproveVendorCommand(id));
        return NoContent();
    }

    // PATCH /api/v1/vendors/{id}/suspend
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/suspend")]
    public async Task<IActionResult> SuspendVendor(
        int id,
        [FromBody] SuspendVendorCommand command)
    {
        // enforce route → command consistency
        var finalCommand = command with { VendorId = id };

        await _mediator.Send(finalCommand);
        return NoContent();
    }
}

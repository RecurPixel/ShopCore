using ShopCore.Application.AdminDashboard.Queries.GetAdminDashboardStats;
using ShopCore.Application.AdminDashboard.Queries.GetRecentOrders;
using ShopCore.Application.AdminDashboard.Queries.GetRevenueStats;

namespace ShopCore.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET /api/v1/admin/dashboard/stats
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _mediator.Send(new GetAdminDashboardStatsQuery());

        return Ok(stats);
    }

    // GET /api/v1/admin/dashboard/recent-orders
    [HttpGet("recent-orders")]
    public async Task<IActionResult> GetRecentOrders([FromQuery] GetRecentOrdersQuery query)
    {
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    // GET /api/v1/admin/dashboard/revenue
    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue([FromQuery] GetRevenueStatsQuery query)
    {
        var revenue = await _mediator.Send(query);
        return Ok(revenue);
    }
}

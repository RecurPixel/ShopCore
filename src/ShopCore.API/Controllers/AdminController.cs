using ShopCore.Application.AdminDashboard.DTOs;
using ShopCore.Application.AdminDashboard.Queries.GetAdminDashboardStats;
using ShopCore.Application.Common.Models;
using ShopCore.Application.Orders.DTOs;
using ShopCore.Application.Orders.Queries.GetAllOrders;
using ShopCore.Application.Orders.Queries.GetOrderById;
using ShopCore.Application.Payouts.Commands.CalculatePendingPayouts;
using ShopCore.Application.Payouts.Commands.CancelPayout;
using ShopCore.Application.Payouts.Commands.CreatePayout;
using ShopCore.Application.Payouts.Commands.ProcessPayout;
using ShopCore.Application.Payouts.DTOs;
using ShopCore.Application.Payouts.Queries.GetAllPayouts;
using ShopCore.Application.Products.Commands.FeatureProduct;
using ShopCore.Application.Products.DTOs;
using ShopCore.Application.Products.Queries.GetAllProducts;
using ShopCore.Application.Reports.DTOs;
using ShopCore.Application.Reports.Queries.GetCustomerAnalytics;
using ShopCore.Application.Reports.Queries.GetProductAnalytics;
using ShopCore.Application.Reports.Queries.GetRevenueReport;
using ShopCore.Application.Reports.Queries.GetVendorPerformance;
using ShopCore.Application.Subscriptions.DTOs;
using ShopCore.Application.Subscriptions.Queries.GetAllSubscriptions;
using ShopCore.Application.Subscriptions.Queries.GetSubscriptionById;
using ShopCore.Application.Users.Commands.DeleteUser;
using ShopCore.Application.Users.Commands.UpdateUser;
using ShopCore.Application.Users.Commands.UpdateUserStatus;
using ShopCore.Application.Users.DTOs;
using ShopCore.Application.Users.Queries.GetAllUsers;
using ShopCore.Application.Users.Queries.GetUserById;
using ShopCore.Application.Vendors.Commands.ActivateVendor;
using ShopCore.Application.Vendors.Commands.ApproveVendor;
using ShopCore.Application.Vendors.Commands.SuspendVendor;
using ShopCore.Application.Vendors.DTOs;
using ShopCore.Application.Vendors.Queries.GetAllVendors;
using ShopCore.Application.Vendors.Queries.GetPendingVendors;

namespace ShopCore.Api.Controllers;

/// <summary>
/// Admin-only operations for managing the platform.
/// </summary>
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/v1/admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // ==================
    // Dashboard
    // ==================

    // GET /api/v1/admin/dashboard
    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardStatsDto>> GetDashboard()
    {
        var stats = await _mediator.Send(new GetAdminDashboardStatsQuery());
        return Ok(stats);
    }

    // ==================
    // Users
    // ==================

    // GET /api/v1/admin/users
    [HttpGet("users")]
    public async Task<ActionResult<PaginatedList<UserDto>>> GetAllUsers(
        [FromQuery] GetAllUsersQuery query)
    {
        var users = await _mediator.Send(query);
        return Ok(users);
    }

    // GET /api/v1/admin/users/{id}
    [HttpGet("users/{id:int}")]
    public async Task<ActionResult<UserDetailDto>> GetUserById(int id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        if (user is null)
            return NotFound();
        return Ok(user);
    }

    // PUT /api/v1/admin/users/{id}
    [HttpPut("users/{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser(
        int id,
        [FromBody] UpdateUserCommand command)
    {
        var finalCommand = command with { Id = id };
        var user = await _mediator.Send(finalCommand);
        return Ok(user);
    }

    // PATCH /api/v1/admin/users/{id}/status
    [HttpPatch("users/{id:int}/status")]
    public async Task<IActionResult> UpdateUserStatus(
        int id,
        [FromBody] UpdateUserStatusCommand command)
    {
        var finalCommand = command with { UserId = id };
        await _mediator.Send(finalCommand);
        return NoContent();
    }

    // DELETE /api/v1/admin/users/{id}
    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }

    // ==================
    // Vendors
    // ==================

    // GET /api/v1/admin/vendors
    [HttpGet("vendors")]
    public async Task<ActionResult<PaginatedList<VendorProfileDto>>> GetAllVendors(
        [FromQuery] GetAllVendorsQuery query)
    {
        var vendors = await _mediator.Send(query);
        return Ok(vendors);
    }

    // GET /api/v1/admin/vendors/pending
    [HttpGet("vendors/pending")]
    public async Task<ActionResult<List<VendorProfileDto>>> GetPendingVendors()
    {
        var vendors = await _mediator.Send(new GetPendingVendorsQuery());
        return Ok(vendors);
    }

    // PATCH /api/v1/admin/vendors/{id}/approve
    [HttpPatch("vendors/{id:int}/approve")]
    public async Task<IActionResult> ApproveVendor(int id)
    {
        await _mediator.Send(new ApproveVendorCommand(id));
        return NoContent();
    }

    // PATCH /api/v1/admin/vendors/{id}/suspend
    [HttpPatch("vendors/{id:int}/suspend")]
    public async Task<IActionResult> SuspendVendor(
        int id,
        [FromBody] SuspendVendorCommand command)
    {
        var finalCommand = command with { VendorId = id };
        await _mediator.Send(finalCommand);
        return NoContent();
    }

    // PATCH /api/v1/admin/vendors/{id}/activate
    [HttpPatch("vendors/{id:int}/activate")]
    public async Task<IActionResult> ActivateVendor(int id)
    {
        await _mediator.Send(new ActivateVendorCommand(id));
        return NoContent();
    }

    // ==================
    // Products
    // ==================

    // GET /api/v1/admin/products
    [HttpGet("products")]
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetAllProducts(
        [FromQuery] GetAllProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    // PATCH /api/v1/admin/products/{id}/feature
    [HttpPatch("products/{id:int}/feature")]
    public async Task<IActionResult> FeatureProduct(
        int id,
        [FromBody] FeatureProductCommand command)
    {
        var finalCommand = command with { ProductId = id };
        await _mediator.Send(finalCommand);
        return NoContent();
    }

    // ==================
    // Orders
    // ==================

    // GET /api/v1/admin/orders
    [HttpGet("orders")]
    public async Task<ActionResult<PaginatedList<OrderDto>>> GetAllOrders(
        [FromQuery] GetAllOrdersQuery query)
    {
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    // GET /api/v1/admin/orders/{id}
    [HttpGet("orders/{id:int}")]
    public async Task<ActionResult<OrderDetailDto>> GetOrderById(int id)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(id));
        if (order is null)
            return NotFound();
        return Ok(order);
    }

    // ==================
    // Subscriptions
    // ==================

    // GET /api/v1/admin/subscriptions
    [HttpGet("subscriptions")]
    public async Task<ActionResult<PaginatedList<SubscriptionDto>>> GetAllSubscriptions(
        [FromQuery] GetAllSubscriptionsQuery query)
    {
        var subscriptions = await _mediator.Send(query);
        return Ok(subscriptions);
    }

    // GET /api/v1/admin/subscriptions/{id}
    [HttpGet("subscriptions/{id:int}")]
    public async Task<ActionResult<SubscriptionDto>> GetSubscriptionById(int id)
    {
        var subscription = await _mediator.Send(new GetSubscriptionByIdQuery(id));
        return Ok(subscription);
    }

    // ==================
    // Payouts
    // ==================

    // GET /api/v1/admin/payouts
    [HttpGet("payouts")]
    public async Task<ActionResult<PaginatedList<PayoutDto>>> GetAllPayouts(
        [FromQuery] GetAllPayoutsQuery query)
    {
        var payouts = await _mediator.Send(query);
        return Ok(payouts);
    }

    // POST /api/v1/admin/payouts/calculate
    [HttpPost("payouts/calculate")]
    public async Task<ActionResult<PendingPayoutSummaryDto>> CalculatePendingPayouts()
    {
        var summary = await _mediator.Send(new CalculatePendingPayoutsCommand());
        return Ok(summary);
    }

    // POST /api/v1/admin/payouts
    [HttpPost("payouts")]
    public async Task<ActionResult<PayoutDto>> CreatePayout(
        [FromBody] CreatePayoutCommand command)
    {
        var payout = await _mediator.Send(command);
        return Created("", payout);
    }

    // PATCH /api/v1/admin/payouts/{id}/process
    [HttpPatch("payouts/{id:int}/process")]
    public async Task<IActionResult> ProcessPayout(int id)
    {
        await _mediator.Send(new ProcessPayoutCommand(id));
        return NoContent();
    }

    // PATCH /api/v1/admin/payouts/{id}/cancel
    [HttpPatch("payouts/{id:int}/cancel")]
    public async Task<IActionResult> CancelPayout(int id)
    {
        await _mediator.Send(new CancelPayoutCommand(id));
        return NoContent();
    }

    // ==================
    // Reports
    // ==================

    // GET /api/v1/admin/reports/revenue
    [HttpGet("reports/revenue")]
    public async Task<ActionResult<RevenueReportDto>> GetRevenueReport(
        [FromQuery] GetRevenueReportQuery query)
    {
        var report = await _mediator.Send(query);
        return Ok(report);
    }

    // GET /api/v1/admin/reports/vendors
    [HttpGet("reports/vendors")]
    public async Task<ActionResult<VendorPerformanceReportDto>> GetVendorPerformance(
        [FromQuery] GetVendorPerformanceQuery query)
    {
        var report = await _mediator.Send(query);
        return Ok(report);
    }

    // GET /api/v1/admin/reports/products
    [HttpGet("reports/products")]
    public async Task<ActionResult<ProductAnalyticsReportDto>> GetProductAnalytics(
        [FromQuery] GetProductAnalyticsQuery query)
    {
        var report = await _mediator.Send(query);
        return Ok(report);
    }

    // GET /api/v1/admin/reports/customers
    [HttpGet("reports/customers")]
    public async Task<ActionResult<CustomerAnalyticsReportDto>> GetCustomerAnalytics(
        [FromQuery] GetCustomerAnalyticsQuery query)
    {
        var report = await _mediator.Send(query);
        return Ok(report);
    }
}

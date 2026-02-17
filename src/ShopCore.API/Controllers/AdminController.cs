using ShopCore.Application.AdminDashboard.DTOs;
using ShopCore.Application.AdminDashboard.Queries.GetAdminDashboardStats;
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

    /// <summary>
    /// Retrieves dashboard.
    /// </summary>
    /// <returns>AdminDashboardStatsDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardStatsDto>> GetDashboard()
    {
        var stats = await _mediator.Send(new GetAdminDashboardStatsQuery());
        return Ok(stats);
    }

    // ==================
    // Users
    // ==================

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;UserDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("users")]
    public async Task<ActionResult<PaginatedList<UserDto>>> GetAllUsers(
        [FromQuery] GetAllUsersQuery query)
    {
        var users = await _mediator.Send(query);
        return Ok(users);
    }

    /// <summary>
    /// Retrieves user.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>UserDetailDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("users/{id:int}")]
    public async Task<ActionResult<UserDetailDto>> GetUserById(int id)
    {
        var user = await _mediator.Send(new GetUserByIdQuery(id));
        if (user is null)
            return NotFound();
        return Ok(user);
    }

    /// <summary>
    /// Updates user.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>UserDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPut("users/{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser(
        int id,
        [FromBody] UpdateUserCommand command)
    {
        var finalCommand = command with { Id = id };
        var user = await _mediator.Send(finalCommand);
        return Ok(user);
    }

    /// <summary>
    /// Updates user status.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPatch("users/{id:int}/status")]
    public async Task<IActionResult> UpdateUserStatus(
        int id,
        [FromBody] UpdateUserStatusCommand command)
    {
        var finalCommand = command with { UserId = id };
        await _mediator.Send(finalCommand);
        return NoContent();
    }

    /// <summary>
    /// Deletes user.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }

    // ==================
    // Vendors
    // ==================

    /// <summary>
    /// Retrieves all vendors.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;VendorProfileDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("vendors")]
    public async Task<ActionResult<PaginatedList<VendorProfileDto>>> GetAllVendors(
        [FromQuery] GetAllVendorsQuery query)
    {
        var vendors = await _mediator.Send(query);
        return Ok(vendors);
    }

    /// <summary>
    /// Retrieves pending vendors.
    /// </summary>
    /// <returns>List&lt;VendorProfileDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("vendors/pending")]
    public async Task<ActionResult<List<VendorProfileDto>>> GetPendingVendors()
    {
        var vendors = await _mediator.Send(new GetPendingVendorsQuery());
        return Ok(vendors);
    }

    /// <summary>
    /// Approves vendor.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPatch("vendors/{id:int}/approve")]
    public async Task<IActionResult> ApproveVendor(int id)
    {
        await _mediator.Send(new ApproveVendorCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Suspends vendor.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPatch("vendors/{id:int}/suspend")]
    public async Task<IActionResult> SuspendVendor(
        int id,
        [FromBody] SuspendVendorCommand command)
    {
        var finalCommand = command with { VendorId = id };
        await _mediator.Send(finalCommand);
        return NoContent();
    }

    /// <summary>
    /// Activates vendor.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPatch("vendors/{id:int}/activate")]
    public async Task<IActionResult> ActivateVendor(int id)
    {
        await _mediator.Send(new ActivateVendorCommand(id));
        return NoContent();
    }

    // ==================
    // Products
    // ==================

    /// <summary>
    /// Retrieves all products.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;ProductDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("products")]
    public async Task<ActionResult<PaginatedList<ProductDto>>> GetAllProducts(
        [FromQuery] GetAllProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    /// <summary>
    /// Partially updates feature product.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <param name="command">The command containing request data</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
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

    /// <summary>
    /// Retrieves all orders.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;OrderDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("orders")]
    public async Task<ActionResult<PaginatedList<OrderDto>>> GetAllOrders(
        [FromQuery] GetAllOrdersQuery query)
    {
        var orders = await _mediator.Send(query);
        return Ok(orders);
    }

    /// <summary>
    /// Retrieves order.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>OrderDetailDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
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

    /// <summary>
    /// Retrieves all subscriptions.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;SubscriptionDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("subscriptions")]
    public async Task<ActionResult<PaginatedList<SubscriptionDto>>> GetAllSubscriptions(
        [FromQuery] GetAllSubscriptionsQuery query)
    {
        var subscriptions = await _mediator.Send(query);
        return Ok(subscriptions);
    }

    /// <summary>
    /// Retrieves subscription.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>SubscriptionDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("subscriptions/{id:int}")]
    public async Task<ActionResult<SubscriptionDto>> GetSubscriptionById(int id)
    {
        var subscription = await _mediator.Send(new GetSubscriptionByIdQuery(id));
        return Ok(subscription);
    }

    // ==================
    // Payouts
    // ==================

    /// <summary>
    /// Retrieves all payouts.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>PaginatedList&lt;PayoutDto&gt;</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("payouts")]
    public async Task<ActionResult<PaginatedList<PayoutDto>>> GetAllPayouts(
        [FromQuery] GetAllPayoutsQuery query)
    {
        var payouts = await _mediator.Send(query);
        return Ok(payouts);
    }

    /// <summary>
    /// Creates or processes calculate pending payouts.
    /// </summary>
    /// <returns>PendingPayoutSummaryDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPost("payouts/calculate")]
    public async Task<ActionResult<PendingPayoutSummaryDto>> CalculatePendingPayouts()
    {
        var summary = await _mediator.Send(new CalculatePendingPayoutsCommand());
        return Ok(summary);
    }

    /// <summary>
    /// Creates a new payout.
    /// </summary>
    /// <param name="command">The command containing request data</param>
    /// <returns>PayoutDto</returns>
    /// <response code="201">Resource created successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPost("payouts")]
    public async Task<ActionResult<PayoutDto>> CreatePayout(
        [FromBody] CreatePayoutCommand command)
    {
        var payout = await _mediator.Send(command);
        return Created("", payout);
    }

    /// <summary>
    /// Processes payout.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPatch("payouts/{id:int}/process")]
    public async Task<IActionResult> ProcessPayout(int id)
    {
        await _mediator.Send(new ProcessPayoutCommand(id));
        return NoContent();
    }

    /// <summary>
    /// Cancels payout.
    /// </summary>
    /// <param name="id">The unique identifier</param>
    /// <returns>Status code indicating success or failure</returns>
    /// <response code="204">Operation completed successfully</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpPatch("payouts/{id:int}/cancel")]
    public async Task<IActionResult> CancelPayout(int id)
    {
        await _mediator.Send(new CancelPayoutCommand(id));
        return NoContent();
    }

    // ==================
    // Reports
    // ==================

    /// <summary>
    /// Retrieves revenue report.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>RevenueReportDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("reports/revenue")]
    public async Task<ActionResult<RevenueReportDto>> GetRevenueReport(
        [FromQuery] GetRevenueReportQuery query)
    {
        var report = await _mediator.Send(query);
        return Ok(report);
    }

    /// <summary>
    /// Retrieves vendor performance.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>VendorPerformanceReportDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("reports/vendors")]
    public async Task<ActionResult<VendorPerformanceReportDto>> GetVendorPerformance(
        [FromQuery] GetVendorPerformanceQuery query)
    {
        var report = await _mediator.Send(query);
        return Ok(report);
    }

    /// <summary>
    /// Retrieves product analytics.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>ProductAnalyticsReportDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    /// <response code="404">Resource not found</response>
    [HttpGet("reports/products")]
    public async Task<ActionResult<ProductAnalyticsReportDto>> GetProductAnalytics(
        [FromQuery] GetProductAnalyticsQuery query)
    {
        var report = await _mediator.Send(query);
        return Ok(report);
    }

    /// <summary>
    /// Retrieves customer analytics.
    /// </summary>
    /// <param name="query">Search query string</param>
    /// <returns>CustomerAnalyticsReportDto</returns>
    /// <response code="200">Returns the requested data</response>
    /// <response code="400">Invalid request parameters</response>
    [HttpGet("reports/customers")]
    public async Task<ActionResult<CustomerAnalyticsReportDto>> GetCustomerAnalytics(
        [FromQuery] GetCustomerAnalyticsQuery query)
    {
        var report = await _mediator.Send(query);
        return Ok(report);
    }
}

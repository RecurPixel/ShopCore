using ShopCore.Application.AdminDashboard.DTOs;

namespace ShopCore.Application.AdminDashboard.Queries.GetAdminDashboardStats;

public class GetAdminDashboardStatsQueryHandler
    : IRequestHandler<GetAdminDashboardStatsQuery, AdminDashboardStatsDto>
{
    private readonly IApplicationDbContext _context;

    public GetAdminDashboardStatsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardStatsDto> Handle(
        GetAdminDashboardStatsQuery request,
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);

        // Use custom date range if provided
        var fromDate = request.FromDate ?? startOfMonth;
        var toDate = request.ToDate ?? DateTime.UtcNow;

        var stats = new AdminDashboardStatsDto
        {
            // Overall counts
            TotalUsers = await _context.Users.CountAsync(cancellationToken),
            TotalVendors = await _context.VendorProfiles.CountAsync(cancellationToken),
            TotalProducts = await _context.Products.CountAsync(p => !p.IsDeleted, cancellationToken),
            TotalOrders = await _context.Orders.CountAsync(cancellationToken),
            TotalSubscriptions = await _context.Subscriptions.CountAsync(cancellationToken),

            // Pending items
            PendingOrders = await _context.Orders
                .CountAsync(o => o.Status == OrderStatus.Pending, cancellationToken),
            PendingVendorApprovals = await _context.VendorProfiles
                .CountAsync(v => v.Status == VendorStatus.PendingApproval, cancellationToken),
            ActiveVendors = await _context.VendorProfiles
                .CountAsync(v => v.Status == VendorStatus.Active, cancellationToken),

            // Today's stats
            OrdersToday = await _context.Orders
                .CountAsync(o => o.CreatedAt >= today, cancellationToken),
            RevenueToday = await _context.Orders
                .Where(o => o.CreatedAt >= today && o.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(o => o.Total, cancellationToken),

            // This week
            OrdersThisWeek = await _context.Orders
                .CountAsync(o => o.CreatedAt >= startOfWeek, cancellationToken),
            RevenueThisWeek = await _context.Orders
                .Where(o => o.CreatedAt >= startOfWeek && o.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(o => o.Total, cancellationToken),

            // This month (or custom date range)
            OrdersThisMonth = await _context.Orders
                .CountAsync(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate, cancellationToken),
            RevenueThisMonth = await _context.Orders
                .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate && o.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(o => o.Total, cancellationToken),

            // Total revenue
            TotalRevenue = await _context.Orders
                .Where(o => o.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(o => o.Total, cancellationToken)
        };

        return stats;
    }
}

using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetCustomerAnalytics;

public class GetCustomerAnalyticsQueryHandler : IRequestHandler<GetCustomerAnalyticsQuery, CustomerAnalyticsReportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCustomerAnalyticsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CustomerAnalyticsReportDto> Handle(GetCustomerAnalyticsQuery request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can view customer analytics");

        var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-12);
        var toDate = request.ToDate ?? DateTime.UtcNow;
        var thisMonthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var totalCustomers = await _context.Users
            .CountAsync(u => u.Role == UserRole.Customer && !u.IsDeleted, ct);

        var newCustomersThisMonth = await _context.Users
            .CountAsync(u => u.Role == UserRole.Customer && u.CreatedAt >= thisMonthStart, ct);

        // Active customers (made an order in the period)
        var activeCustomers = await _context.Orders
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
            .Select(o => o.UserId)
            .Distinct()
            .CountAsync(ct);

        // Average order value
        var averageOrderValue = await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
            .AverageAsync(o => (decimal?)o.Total, ct) ?? 0;

        // Calculate retention rate (customers who made repeat purchases)
        var repeatCustomers = await _context.Orders
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
            .GroupBy(o => o.UserId)
            .Where(g => g.Count() > 1)
            .CountAsync(ct);

        var retentionRate = activeCustomers > 0
            ? ((decimal)repeatCustomers / activeCustomers) * 100
            : 0;

        // Customer segments by spending
        var customerSpending = await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
            .GroupBy(o => o.UserId)
            .Select(g => new { UserId = g.Key, TotalSpent = g.Sum(o => o.Total) })
            .ToListAsync(ct);

        var segments = new List<CustomerSegmentDto>();

        segments.Add(new CustomerSegmentDto
        {
            Segment = "High Value (>10K)",
            CustomerCount = customerSpending.Count(c => c.TotalSpent > 10000),
            TotalSpent = customerSpending.Where(c => c.TotalSpent > 10000).Sum(c => c.TotalSpent)
        });

        segments.Add(new CustomerSegmentDto
        {
            Segment = "Medium Value (5K-10K)",
            CustomerCount = customerSpending.Count(c => c.TotalSpent >= 5000 && c.TotalSpent <= 10000),
            TotalSpent = customerSpending.Where(c => c.TotalSpent >= 5000 && c.TotalSpent <= 10000).Sum(c => c.TotalSpent)

        });
        segments.Add(new CustomerSegmentDto
        {
            Segment = "Low Value (<5K)",
            CustomerCount = customerSpending.Count(c => c.TotalSpent < 5000),
            TotalSpent = customerSpending.Where(c => c.TotalSpent < 5000).Sum(c => c.TotalSpent)
        });

        return new CustomerAnalyticsReportDto
        {
            TotalCustomers = totalCustomers,
            NewCustomersThisMonth = newCustomersThisMonth,
            ActiveCustomers = activeCustomers,
            AverageOrderValue = Math.Round(averageOrderValue, 2),
            CustomerRetentionRate = Math.Round(retentionRate, 2),
            CustomerSegments = segments
        };
    }
}

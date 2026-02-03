using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetRevenueReport;

public class GetRevenueReportQueryHandler : IRequestHandler<GetRevenueReportQuery, RevenueReportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetRevenueReportQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<RevenueReportDto> Handle(GetRevenueReportQuery request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can view revenue reports");

        var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-12);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        // Calculate order revenue
        var orderRevenue = await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
            .SumAsync(o => o.Total, ct);

        // Calculate subscription revenue
        var subscriptionRevenue = await _context.SubscriptionInvoices
            .Where(i => i.Status == InvoiceStatus.Paid)
            .Where(i => i.GeneratedAt >= fromDate && i.GeneratedAt <= toDate)
            .SumAsync(i => i.Total, ct);

        var totalRevenue = orderRevenue + subscriptionRevenue;

        // Calculate previous period for growth comparison
        var periodLength = (toDate - fromDate).TotalDays;
        var previousFromDate = fromDate.AddDays(-periodLength);
        var previousToDate = fromDate.AddDays(-1);

        var previousOrderRevenue = await _context.Orders
            .Where(o => o.Status == OrderStatus.Delivered)
            .Where(o => o.CreatedAt >= previousFromDate && o.CreatedAt <= previousToDate)
            .SumAsync(o => o.Total, ct);

        var previousSubscriptionRevenue = await _context.SubscriptionInvoices
            .Where(i => i.Status == InvoiceStatus.Paid)
            .Where(i => i.GeneratedAt >= previousFromDate && i.GeneratedAt <= previousToDate)
            .SumAsync(i => i.Total, ct);

        var previousTotal = previousOrderRevenue + previousSubscriptionRevenue;
        var growthPercentage = previousTotal > 0
            ? ((totalRevenue - previousTotal) / previousTotal) * 100
            : 0;

        // Revenue by period
        var revenueByPeriod = await GetRevenueByPeriod(fromDate, toDate, request.Period, ct);

        return new RevenueReportDto(
            totalRevenue,
            orderRevenue,
            subscriptionRevenue,
            Math.Round(growthPercentage, 2),
            revenueByPeriod
        );
    }

    private async Task<List<RevenueByPeriodDto>> GetRevenueByPeriod(
        DateTime fromDate, DateTime toDate, string period, CancellationToken ct)
    {
        var result = new List<RevenueByPeriodDto>();
        var current = fromDate;

        while (current <= toDate)
        {
            DateTime periodEnd;
            string periodLabel;

            switch (period.ToLower())
            {
                case "day":
                    periodEnd = current.AddDays(1).AddSeconds(-1);
                    periodLabel = current.ToString("dd MMM");
                    break;
                case "week":
                    periodEnd = current.AddDays(7).AddSeconds(-1);
                    periodLabel = $"Week of {current:dd MMM}";
                    break;
                case "month":
                default:
                    periodEnd = current.AddMonths(1).AddSeconds(-1);
                    periodLabel = current.ToString("MMM yyyy");
                    break;
            }

            var orderRev = await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .Where(o => o.CreatedAt >= current && o.CreatedAt <= periodEnd)
                .SumAsync(o => o.Total, ct);

            var subRev = await _context.SubscriptionInvoices
                .Where(i => i.Status == InvoiceStatus.Paid)
                .Where(i => i.GeneratedAt >= current && i.GeneratedAt <= periodEnd)
                .SumAsync(i => i.Total, ct);

            result.Add(new RevenueByPeriodDto(periodLabel, orderRev + subRev));

            current = period.ToLower() switch
            {
                "day" => current.AddDays(1),
                "week" => current.AddDays(7),
                _ => current.AddMonths(1)
            };
        }

        return result;
    }
}

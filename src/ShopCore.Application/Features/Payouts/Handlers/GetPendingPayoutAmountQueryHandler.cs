using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetPendingPayoutAmount;

public class GetPendingPayoutAmountQueryHandler : IRequestHandler<GetPendingPayoutAmountQuery, PendingPayoutDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPendingPayoutAmountQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PendingPayoutDto> Handle(
        GetPendingPayoutAmountQuery request,
        CancellationToken ct)
    {
        var vendorId = _currentUser.VendorId;

        // Get the last payout date
        var lastPayout = await _context.VendorPayouts
            .Where(p => p.VendorId == vendorId && p.Status == PayoutStatus.Paid)
            .OrderByDescending(p => p.PeriodEnd)
            .FirstOrDefaultAsync(ct);

        var lastPayoutDate = lastPayout?.PeriodEnd ?? DateTime.MinValue;

        // Use request dates if provided, otherwise use lastPayoutDate
        var fromDate = request.FromDate ?? lastPayoutDate;
        var toDate = request.ToDate ?? DateTime.UtcNow;

        // Calculate pending orders (completed orders not yet in a payout)
        var ordersQuery = _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.Items.Any(i => i.VendorId == vendorId))
            .Where(o => o.Status == OrderStatus.Delivered)
            .Where(o => o.CreatedAt > fromDate && o.CreatedAt <= toDate);

        var pendingOrders = await ordersQuery.ToListAsync(ct);

        var orderCount = pendingOrders.Count;
        var earliestOrderDate = pendingOrders.MinBy(o => o.CreatedAt)?.CreatedAt;

        // Calculate pending deliveries
        var pendingDeliveries = await _context.Deliveries
            .AsNoTracking()
            .Where(d => d.Subscription.VendorId == vendorId)
            .Where(d => d.Status == DeliveryStatus.Delivered)
            .Where(d => d.ActualDeliveryDate > fromDate && d.ActualDeliveryDate <= toDate)
            .CountAsync(ct);

        // Calculate total amount from orders
        var orderAmount = pendingOrders
            .SelectMany(o => o.Items.Where(i => i.VendorId == vendorId))
            .Sum(i => i.Subtotal);

        // Calculate total from delivered subscriptions
        var deliveryAmount = await _context.Deliveries
            .AsNoTracking()
            .Where(d => d.Subscription.VendorId == vendorId)
            .Where(d => d.Status == DeliveryStatus.Delivered)
            .Where(d => d.ActualDeliveryDate > fromDate && d.ActualDeliveryDate <= toDate)
            .SumAsync(d => d.TotalAmount, ct);

        var totalPendingAmount = orderAmount + deliveryAmount;

        // Next payout date (assuming weekly payouts on Monday)
        var nextPayoutDate = GetNextPayoutDate();

        return new PendingPayoutDto
        {
            Amount = totalPendingAmount,
            OrderCount = orderCount,
            DeliveryCount = pendingDeliveries,
            EarliestOrderDate = earliestOrderDate,
            NextPayoutDate = nextPayoutDate
        };
    }

    private static DateTime GetNextPayoutDate()
    {
        var today = DateTime.UtcNow.Date;
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        if (daysUntilMonday == 0) daysUntilMonday = 7;
        return today.AddDays(daysUntilMonday);
    }
}

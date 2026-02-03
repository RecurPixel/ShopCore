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
            .Where(p => p.VendorId == vendorId && p.Status == PayoutStatus.Completed)
            .OrderByDescending(p => p.PeriodEnd)
            .FirstOrDefaultAsync(ct);

        var lastPayoutDate = lastPayout?.PeriodEnd ?? DateTime.MinValue;

        // Calculate pending orders (completed orders not yet in a payout)
        var pendingOrders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.Items.Any(i => i.VendorId == vendorId))
            .Where(o => o.Status == OrderStatus.Delivered && o.CreatedAt > lastPayoutDate)
            .ToListAsync(ct);

        var orderCount = pendingOrders.Count;
        var earliestOrderDate = pendingOrders.MinBy(o => o.CreatedAt)?.CreatedAt;

        // Calculate pending deliveries
        var pendingDeliveries = await _context.Deliveries
            .AsNoTracking()
            .Where(d => d.Subscription.VendorId == vendorId)
            .Where(d => d.Status == DeliveryStatus.Delivered && d.ActualDeliveryDate > lastPayoutDate)
            .CountAsync(ct);

        // Calculate total amount from orders
        var orderAmount = pendingOrders
            .SelectMany(o => o.Items.Where(i => i.VendorId == vendorId))
            .Sum(i => i.Subtotal);

        // Calculate total from delivered subscriptions
        var deliveryAmount = await _context.Deliveries
            .AsNoTracking()
            .Where(d => d.Subscription.VendorId == vendorId)
            .Where(d => d.Status == DeliveryStatus.Delivered && d.ActualDeliveryDate > lastPayoutDate)
            .SumAsync(d => d.TotalAmount, ct);

        var totalPendingAmount = orderAmount + deliveryAmount;

        // Next payout date (assuming weekly payouts on Monday)
        var nextPayoutDate = GetNextPayoutDate();

        return new PendingPayoutDto(
            totalPendingAmount,
            orderCount,
            pendingDeliveries,
            earliestOrderDate,
            nextPayoutDate
        );
    }

    private static DateTime GetNextPayoutDate()
    {
        var today = DateTime.UtcNow.Date;
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        if (daysUntilMonday == 0) daysUntilMonday = 7;
        return today.AddDays(daysUntilMonday);
    }
}

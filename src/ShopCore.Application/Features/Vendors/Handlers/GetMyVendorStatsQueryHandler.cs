using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetMyVendorStats;

public class GetMyVendorStatsQueryHandler : IRequestHandler<GetMyVendorStatsQuery, VendorStatsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyVendorStatsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<VendorStatsDto> Handle(
        GetMyVendorStatsQuery request,
        CancellationToken cancellationToken)
    {
        var vendorId = _currentUser.VendorId;
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        // Evaluate pending payout condition separately to avoid untranslatable nested subquery
        var hasPaidPayout = await _context.VendorPayouts
            .AnyAsync(vp => vp.VendorId == vendorId && vp.Status == PayoutStatus.Paid, cancellationToken);

        var stats = new VendorStatsDto
        {
            // Product stats
            TotalProducts = await _context.Products
                .CountAsync(p => p.VendorId == vendorId && !p.IsDeleted, cancellationToken),
            ActiveProducts = await _context.Products
                .CountAsync(p => p.VendorId == vendorId && !p.IsDeleted
                    && p.Status == ProductStatus.Active, cancellationToken),

            // Order stats
            TotalOrders = await _context.OrderItems
                .CountAsync(oi => oi.VendorId == vendorId, cancellationToken),
            PendingOrders = await _context.OrderItems
                .CountAsync(oi => oi.VendorId == vendorId
                    && oi.Status == OrderItemStatus.Pending, cancellationToken),
            CompletedOrders = await _context.OrderItems
                .CountAsync(oi => oi.VendorId == vendorId
                    && oi.Status == OrderItemStatus.Delivered, cancellationToken),

            // Subscription stats
            TotalSubscriptions = await _context.Subscriptions
                .CountAsync(s => s.VendorId == vendorId, cancellationToken),
            ActiveSubscriptions = await _context.Subscriptions
                .CountAsync(s => s.VendorId == vendorId
                    && s.Status == SubscriptionStatus.Active, cancellationToken),

            // Revenue stats (inline arithmetic avoids unmapped computed property)
            TotalRevenue = await _context.OrderItems
                .Where(oi => oi.VendorId == vendorId
                    && oi.Order.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice * (1 - oi.CommissionRate / 100), cancellationToken),
            RevenueThisMonth = await _context.OrderItems
                .Where(oi => oi.VendorId == vendorId
                    && oi.Order.PaymentStatus == PaymentStatus.Paid
                    && oi.Order.CreatedAt >= startOfMonth)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice * (1 - oi.CommissionRate / 100), cancellationToken),

            // Pending payout
            PendingPayout = hasPaidPayout ? 0m : await _context.OrderItems
                .Where(oi => oi.VendorId == vendorId
                    && oi.Order.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(oi => oi.Quantity * oi.UnitPrice * (1 - oi.CommissionRate / 100), cancellationToken),

            // Review stats
            Rating = await _context.Reviews
                .Where(r => r.Product.VendorId == vendorId)
                .AverageAsync(r => (decimal?)r.Rating, cancellationToken) ?? 0,
            TotalReviews = await _context.Reviews
                .CountAsync(r => r.Product.VendorId == vendorId, cancellationToken)
        };

        return stats;
    }
}

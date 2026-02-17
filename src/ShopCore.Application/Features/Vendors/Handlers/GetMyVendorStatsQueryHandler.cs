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
        var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var stats = new VendorStatsDto
        {
            // Product stats
            TotalProducts = await _context.Products
                .CountAsync(p => p.VendorId == _currentUser.VendorId && !p.IsDeleted, cancellationToken),
            ActiveProducts = await _context.Products
                .CountAsync(p => p.VendorId == _currentUser.VendorId && !p.IsDeleted
                    && p.Status == ProductStatus.Active, cancellationToken),

            // Order stats
            TotalOrders = await _context.OrderItems
                .CountAsync(oi => oi.VendorId == _currentUser.VendorId, cancellationToken),
            PendingOrders = await _context.OrderItems
                .CountAsync(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Status == OrderItemStatus.Pending, cancellationToken),
            CompletedOrders = await _context.OrderItems
                .CountAsync(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Status == OrderItemStatus.Delivered, cancellationToken),

            // Subscription stats
            TotalSubscriptions = await _context.Subscriptions
                .CountAsync(s => s.VendorId == _currentUser.VendorId, cancellationToken),
            ActiveSubscriptions = await _context.Subscriptions
                .CountAsync(s => s.VendorId == _currentUser.VendorId
                    && s.Status == SubscriptionStatus.Active, cancellationToken),

            // Revenue stats
            TotalRevenue = await _context.OrderItems
                .Where(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Order.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(oi => oi.VendorAmount, cancellationToken),
            RevenueThisMonth = await _context.OrderItems
                .Where(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Order.PaymentStatus == PaymentStatus.Paid
                    && oi.Order.CreatedAt >= startOfMonth)
                .SumAsync(oi => oi.VendorAmount, cancellationToken),

            // Pending payout
            PendingPayout = await _context.OrderItems
                .Where(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Order.PaymentStatus == PaymentStatus.Paid
                    && !_context.VendorPayouts.Any(vp =>
                        vp.VendorId == _currentUser.VendorId
                        && vp.Status == PayoutStatus.Paid))
                .SumAsync(oi => oi.VendorAmount, cancellationToken),

            // Review stats
            Rating = await _context.Reviews
                .Where(r => r.Product.VendorId == _currentUser.VendorId)
                .AverageAsync(r => (decimal?)r.Rating, cancellationToken) ?? 0,
            TotalReviews = await _context.Reviews
                .CountAsync(r => r.Product.VendorId == _currentUser.VendorId, cancellationToken)
        };

        return stats;
    }
}
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
        var today = DateTime.UtcNow.Date;
        var startOfMonth = new DateTime(today.Year, today.Month, 1);
        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);

        var stats = new VendorStatsDto
        {
            // Product stats
            TotalProducts = await _context.Products
                .CountAsync(p => p.VendorId == _currentUser.VendorId, cancellationToken),
            ActiveProducts = await _context.Products
                .CountAsync(p => p.VendorId == _currentUser.VendorId
                    && p.Status == ProductStatus.Active, cancellationToken),
            OutOfStockProducts = await _context.Products
                .CountAsync(p => p.VendorId == _currentUser.VendorId
                    && p.TrackInventory && p.StockQuantity == 0, cancellationToken),

            // Order stats
            TotalOrders = await _context.OrderItems
                .CountAsync(oi => oi.VendorId == _currentUser.VendorId, cancellationToken),
            OrdersToday = await _context.OrderItems
                .CountAsync(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Order.CreatedAt >= today, cancellationToken),
            OrdersThisWeek = await _context.OrderItems
                .CountAsync(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Order.CreatedAt >= startOfWeek, cancellationToken),
            OrdersThisMonth = await _context.OrderItems
                .CountAsync(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Order.CreatedAt >= startOfMonth, cancellationToken),

            // Revenue stats
            TotalRevenue = await _context.OrderItems
                .Where(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Order.PaymentStatus == PaymentStatus.Paid)
                .SumAsync(oi => oi.VendorAmount, cancellationToken),
            RevenueToday = await _context.OrderItems
                .Where(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Order.PaymentStatus == PaymentStatus.Paid
                    && oi.Order.CreatedAt >= today)
                .SumAsync(oi => oi.VendorAmount, cancellationToken),
            RevenueThisMonth = await _context.OrderItems
                .Where(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Order.PaymentStatus == PaymentStatus.Paid
                    && oi.Order.CreatedAt >= startOfMonth)
                .SumAsync(oi => oi.VendorAmount, cancellationToken),

            // Subscription stats (if private features enabled)
            TotalSubscriptions = await _context.Subscriptions
                .CountAsync(s => s.VendorId == _currentUser.VendorId, cancellationToken),
            ActiveSubscriptions = await _context.Subscriptions
                .CountAsync(s => s.VendorId == _currentUser.VendorId
                    && s.Status == SubscriptionStatus.Active, cancellationToken),

            // Customer stats
            TotalCustomers = await _context.OrderItems
                .Where(oi => oi.VendorId == _currentUser.VendorId)
                .Select(oi => oi.Order.UserId)
                .Distinct()
                .CountAsync(cancellationToken),

            // Pending amounts
            PendingPayout = await _context.OrderItems
                .Where(oi => oi.VendorId == _currentUser.VendorId
                    && oi.Order.PaymentStatus == PaymentStatus.Paid
                    && !_context.VendorPayouts.Any(vp =>
                        vp.VendorId == _currentUser.VendorId
                        && vp.Status == PayoutStatus.Paid))
                .SumAsync(oi => oi.VendorAmount, cancellationToken)
        };

        return stats;
    }
}
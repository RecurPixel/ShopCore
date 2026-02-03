using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetProductAnalytics;

public class GetProductAnalyticsQueryHandler : IRequestHandler<GetProductAnalyticsQuery, ProductAnalyticsReportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetProductAnalyticsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ProductAnalyticsReportDto> Handle(GetProductAnalyticsQuery request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can view product analytics");

        var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-12);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        var totalProducts = await _context.Products.CountAsync(p => !p.IsDeleted, ct);
        var activeProducts = await _context.Products.CountAsync(p => !p.IsDeleted && p.IsActive, ct);
        var outOfStockProducts = await _context.Products
            .CountAsync(p => !p.IsDeleted && p.StockQuantity == 0, ct);

        // Top selling products
        var topProducts = await _context.OrderItems
            .Where(oi => oi.Order.Status == OrderStatus.Delivered)
            .Where(oi => oi.Order.CreatedAt >= fromDate && oi.Order.CreatedAt <= toDate)
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
            .Select(g => new TopProductDto(
                g.Key.ProductId,
                g.Key.Name,
                g.Sum(oi => oi.Quantity),
                g.Sum(oi => oi.Subtotal)
            ))
            .OrderByDescending(p => p.Revenue)
            .Take(request.Top)
            .ToListAsync(ct);

        // Sales by category
        var salesByCategory = await _context.OrderItems
            .Where(oi => oi.Order.Status == OrderStatus.Delivered)
            .Where(oi => oi.Order.CreatedAt >= fromDate && oi.Order.CreatedAt <= toDate)
            .GroupBy(oi => new { oi.Product.CategoryId, oi.Product.Category.Name })
            .Select(g => new CategorySalesDto(
                g.Key.CategoryId,
                g.Key.Name,
                g.Select(oi => oi.ProductId).Distinct().Count(),
                g.Sum(oi => oi.Subtotal)
            ))
            .OrderByDescending(c => c.Revenue)
            .ToListAsync(ct);

        return new ProductAnalyticsReportDto(
            totalProducts,
            activeProducts,
            outOfStockProducts,
            topProducts,
            salesByCategory
        );
    }
}

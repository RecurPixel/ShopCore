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
        var activeProducts = await _context.Products.CountAsync(p => !p.IsDeleted && p.Status == ProductStatus.Active, ct);
        var outOfStockProducts = await _context.Products
            .CountAsync(p => !p.IsDeleted && p.StockQuantity == 0, ct);

        // Top selling products
        var topProducts = await _context.OrderItems
            .Where(oi => oi.Order.Status == OrderStatus.Delivered)
            .Where(oi => oi.Order.CreatedAt >= fromDate && oi.Order.CreatedAt <= toDate)
            .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
            .Select(g => new TopProductDto
            {
                ProductId = g.Key.ProductId,
                ProductName = g.Key.Name,
                SoldCount = g.Sum(oi => oi.Quantity),
                Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .OrderByDescending(p => p.Revenue)
            .Take(request.Top)
            .ToListAsync(ct);

        // Sales by category
        var salesByCategory = await _context.OrderItems
            .Where(oi => oi.Order.Status == OrderStatus.Delivered)
            .Where(oi => oi.Order.CreatedAt >= fromDate && oi.Order.CreatedAt <= toDate)
            .GroupBy(oi => new { oi.Product.CategoryId, oi.Product.Category.Name })
            .Select(g => new CategorySalesDto
            {
                CategoryId = g.Key.CategoryId,
                CategoryName = g.Key.Name,
                ProductCount = g.Select(oi => oi.ProductId).Distinct().Count(),
                Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .OrderByDescending(c => c.Revenue)
            .ToListAsync(ct);

        return new ProductAnalyticsReportDto
        {
            TotalProducts = totalProducts,
            ActiveProducts = activeProducts,
            OutOfStockProducts = outOfStockProducts,
            TopSellingProducts = topProducts,
            SalesByCategory = salesByCategory
        };
    }
}

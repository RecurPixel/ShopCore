using ShopCore.Application.Reports.DTOs;

namespace ShopCore.Application.Reports.Queries.GetVendorPerformance;

public class GetVendorPerformanceQueryHandler : IRequestHandler<GetVendorPerformanceQuery, VendorPerformanceReportDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorPerformanceQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<VendorPerformanceReportDto> Handle(GetVendorPerformanceQuery request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can view vendor performance reports");

        var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-12);
        var toDate = request.ToDate ?? DateTime.UtcNow;

        var totalVendors = await _context.VendorProfiles.CountAsync(ct);
        var activeVendors = await _context.VendorProfiles
            .CountAsync(v => v.Status == VendorStatus.Active, ct);

        // Get top vendors by revenue
        var vendorSales = await _context.OrderItems
            .Where(oi => oi.Order.Status == OrderStatus.Delivered)
            .Where(oi => oi.Order.CreatedAt >= fromDate && oi.Order.CreatedAt <= toDate)
            .GroupBy(oi => oi.VendorId)
            .Select(g => new
            {
                VendorId = g.Key,
                Revenue = g.Sum(oi => oi.Subtotal),
                OrderCount = g.Select(oi => oi.OrderId).Distinct().Count()
            })
            .OrderByDescending(x => x.Revenue)
            .Take(request.Top)
            .ToListAsync(ct);

        var vendorIds = vendorSales.Select(v => v.VendorId).ToList();
        var vendors = await _context.VendorProfiles
            .Where(v => vendorIds.Contains(v.Id))
            .ToDictionaryAsync(v => v.Id, ct);

        var topVendors = vendorSales.Select(vs => new TopVendorDto(
            vs.VendorId,
            vendors.TryGetValue(vs.VendorId, out var v) ? v.BusinessName : "Unknown",
            vs.Revenue,
            vs.OrderCount,
            vendors.TryGetValue(vs.VendorId, out var vr) ? vr.AverageRating : 0
        )).ToList();

        var averageRating = await _context.VendorProfiles
            .Where(v => v.Status == VendorStatus.Active && v.TotalReviews > 0)
            .AverageAsync(v => (decimal?)v.AverageRating, ct) ?? 0;

        return new VendorPerformanceReportDto(
            totalVendors,
            activeVendors,
            topVendors,
            Math.Round(averageRating, 2)
        );
    }
}

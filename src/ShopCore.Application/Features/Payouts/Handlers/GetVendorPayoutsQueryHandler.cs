using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetVendorPayouts;

public class GetVendorPayoutsQueryHandler
    : IRequestHandler<GetVendorPayoutsQuery, PaginatedList<VendorPayoutDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorPayoutsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<VendorPayoutDto>> Handle(
        GetVendorPayoutsQuery request,
        CancellationToken ct)
    {
        var query = _context.VendorPayouts
            .AsNoTracking()
            .Include(p => p.Vendor)
            .Where(p => p.VendorId == _currentUser.VendorId);

        // Apply filters
        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(p => p.Status.ToString() == request.Status);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(p => p.CreatedAt <= request.ToDate.Value);
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new VendorPayoutDto
            {
                Id = p.Id,
                VendorId = p.VendorId,
                VendorName = p.Vendor.BusinessName,
                PeriodFrom = p.PeriodStart,
                PeriodTo = p.PeriodEnd,
                GrossAmount = p.TotalSales,
                PlatformFee = p.CommissionAmount,
                NetAmount = p.NetPayout,
                Status = p.Status.ToString(),
                PayoutMethod = p.PayoutMethod.ToString(),
                PayoutTransactionId = p.PayoutTransactionId,
                ProcessedAt = p.PaidAt
            })
            .ToListAsync(ct);

        return new PaginatedList<VendorPayoutDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}

using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetVendorPayouts;

public class GetVendorPayoutsQueryHandler
    : IRequestHandler<GetVendorPayoutsQuery, List<VendorPayoutDto>>
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

    public async Task<List<VendorPayoutDto>> Handle(
        GetVendorPayoutsQuery request,
        CancellationToken ct)
    {
        var payouts = await _context.VendorPayouts
            .AsNoTracking()
            .Where(p => p.VendorId == _currentUser.VendorId)
            .Include(p => p.Vendor)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(ct);

        return payouts.Select(p => new VendorPayoutDto
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
            PayoutMethod = p.PayoutMethod?.ToString(),
            PayoutTransactionId = p.PayoutTransactionId,
            ProcessedAt = p.PaidAt
        }).ToList();
    }
}

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

        return payouts.Select(p => new VendorPayoutDto(
            p.Id,
            p.VendorId,
            p.Vendor.BusinessName,
            p.PeriodStart,
            p.PeriodEnd,
            p.TotalSales,
            p.CommissionAmount,
            p.NetPayout,
            p.Status,
            p.TransactionId,
            p.PaidAt,
            null
        )).ToList();
    }
}

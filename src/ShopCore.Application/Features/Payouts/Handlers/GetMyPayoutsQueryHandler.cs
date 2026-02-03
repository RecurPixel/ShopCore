using ShopCore.Application.Common.Models;
using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetMyPayouts;

public class GetMyPayoutsQueryHandler : IRequestHandler<GetMyPayoutsQuery, PaginatedList<PayoutDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyPayoutsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<PayoutDto>> Handle(
        GetMyPayoutsQuery request,
        CancellationToken ct)
    {
        var query = _context.VendorPayouts
            .AsNoTracking()
            .Where(p => p.VendorId == _currentUser.VendorId)
            .Include(p => p.Vendor)
            .AsQueryable();

        // Filter by status
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<PayoutStatus>(request.Status, true, out var status))
        {
            query = query.Where(p => p.Status == status);
        }

        var totalCount = await query.CountAsync(ct);

        var payouts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var items = payouts.Select(p => new PayoutDto(
            p.Id,
            p.VendorId,
            p.Vendor.BusinessName,
            p.NetPayout,
            p.Status.ToString(),
            p.TransactionReference,
            null,
            p.CreatedAt,
            p.PaidAt
        )).ToList();

        return new PaginatedList<PayoutDto>(items, totalCount, request.Page, request.PageSize);
    }
}

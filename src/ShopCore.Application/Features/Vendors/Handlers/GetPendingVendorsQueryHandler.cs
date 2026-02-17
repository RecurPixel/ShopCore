using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetPendingVendors;

public class GetPendingVendorsQueryHandler
    : IRequestHandler<GetPendingVendorsQuery, PaginatedList<VendorProfileDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingVendorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<VendorProfileDto>> Handle(
        GetPendingVendorsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.VendorProfiles
            .AsNoTracking()
            .Include(v => v.User)
            .Where(v => v.Status == VendorStatus.PendingApproval);

        // Apply search filter
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(v =>
                v.BusinessName.Contains(request.Search) ||
                v.User.Email.Contains(request.Search) ||
                v.User.PhoneNumber.Contains(request.Search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(v => v.CreatedAt) // Oldest first for review queue
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(v => new VendorProfileDto
            {
                Id = v.Id,
                UserId = v.UserId,
                BusinessName = v.BusinessName,
                BusinessDescription = v.BusinessDescription,
                BusinessLogo = v.BusinessLogo,
                BusinessAddress = v.BusinessAddress,
                GstNumber = v.GstNumber,
                PanNumber = v.PanNumber,
                Email = v.User.Email,
                PhoneNumber = v.User.PhoneNumber,
                Status = v.Status.ToString(),
                CreatedAt = v.CreatedAt,
                DaysSinceSubmission = (int)(DateTime.UtcNow - v.CreatedAt).TotalDays
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<VendorProfileDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}


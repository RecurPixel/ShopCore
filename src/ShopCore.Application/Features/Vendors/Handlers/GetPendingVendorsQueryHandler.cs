using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetPendingVendors;

public class GetPendingVendorsQueryHandler
    : IRequestHandler<GetPendingVendorsQuery, List<VendorProfileDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingVendorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VendorProfileDto>> Handle(
        GetPendingVendorsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.VendorProfiles
            .AsNoTracking()
            .Include(v => v.User)
            .Where(v => v.Status == VendorStatus.PendingApproval)
            .OrderBy(v => v.CreatedAt) // Oldest first for review queue
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
    }
}


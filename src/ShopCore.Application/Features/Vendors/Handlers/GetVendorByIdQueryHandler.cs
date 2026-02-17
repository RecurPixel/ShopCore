using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorById;

public class GetVendorByIdQueryHandler : IRequestHandler<GetVendorByIdQuery, VendorPublicProfileDto?>
{
    private readonly IApplicationDbContext _context;

    public GetVendorByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VendorPublicProfileDto?> Handle(
        GetVendorByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.VendorProfiles
            .AsNoTracking()
            .Include(v => v.ServiceAreas)
            .Where(v => v.Id == request.Id && v.Status == VendorStatus.Active)
            .Select(v => new VendorPublicProfileDto
            {
                Id = v.Id,
                BusinessName = v.BusinessName,
                Description = v.BusinessDescription,
                LogoUrl = v.BusinessLogo,
                Rating = v.AverageRating,
                ReviewCount = v.TotalReviews,
                ServiceAreas = v.ServiceAreas.Select(sa => sa.AreaName).ToList(),
                IsOpen = v.Status == VendorStatus.Active
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}

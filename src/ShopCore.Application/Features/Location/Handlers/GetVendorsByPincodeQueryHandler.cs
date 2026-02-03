using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Queries.GetVendorsByPincode;

public class GetVendorsByPincodeQueryHandler : IRequestHandler<GetVendorsByPincodeQuery, List<NearbyVendorDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVendorsByPincodeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<NearbyVendorDto>> Handle(GetVendorsByPincodeQuery request, CancellationToken ct)
    {
        // Validate pincode format (6 digits for India)
        if (string.IsNullOrEmpty(request.Pincode) || request.Pincode.Length != 6)
            throw new BadRequestException("Invalid pincode format. Must be 6 digits.");

        // Find vendors serving this pincode
        var serviceAreas = await _context.VendorServiceAreas
            .AsNoTracking()
            .Where(sa => sa.IsActive && sa.Pincodes.Contains(request.Pincode))
            .Include(sa => sa.Vendor)
            .Where(sa => sa.Vendor.Status == VendorStatus.Active)
            .ToListAsync(ct);

        var vendorIds = serviceAreas.Select(sa => sa.VendorId).Distinct().ToList();

        var vendors = await _context.VendorProfiles
            .AsNoTracking()
            .Where(v => vendorIds.Contains(v.Id))
            .Include(v => v.ServiceAreas.Where(sa => sa.IsActive))
            .ToListAsync(ct);

        return vendors
            .Select(v => new NearbyVendorDto(
                v.Id,
                v.BusinessName,
                v.BusinessLogo,
                v.AverageRating,
                v.TotalReviews,
                0, // Distance not applicable for pincode search
                v.Status == VendorStatus.Active,
                v.ServiceAreas.Select(sa => sa.AreaName).ToList()
            ))
            .OrderByDescending(v => v.Rating)
            .ToList();
    }
}

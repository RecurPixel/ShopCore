using ShopCore.Application.Vendors.DTOs;
using ShopCore.Application.Vendors.Queries.SearchVendorsByLocation;

namespace ShopCore.Application.Vendors.Handlers;

public class SearchVendorsByLocationQueryHandler
    : IRequestHandler<SearchVendorsByLocationQuery, List<VendorSearchResultDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchVendorsByLocationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<VendorSearchResultDto>> Handle(
        SearchVendorsByLocationQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Pincode) && !request.Latitude.HasValue)
        {
            throw new InvalidOperationException("Either Pincode or Latitude/Longitude must be provided");
        }

        IQueryable<VendorProfile> vendorsQuery;

        if (!string.IsNullOrEmpty(request.Pincode))
        {
            // Search by pincode - find vendors whose service areas include this pincode
            var vendorIds = await _context.VendorServiceAreas
                .Where(sa => sa.IsActive && sa.Pincodes.Contains(request.Pincode))
                .Select(sa => sa.VendorId)
                .Distinct()
                .ToListAsync(cancellationToken);

            vendorsQuery = _context.VendorProfiles
                .Where(v => vendorIds.Contains(v.Id) && v.Status == VendorStatus.Active);
        }
        else
        {
            // For now, fall back to all active vendors
            // TODO: Implement geofencing when GeoJsonPolygon is populated
            vendorsQuery = _context.VendorProfiles
                .Where(v => v.Status == VendorStatus.Active);
        }

        // Filter by category if specified
        if (request.CategoryId.HasValue)
        {
            var vendorIdsWithCategory = await _context.Products
                .Where(p => p.CategoryId == request.CategoryId.Value)
                .Select(p => p.VendorId)
                .Distinct()
                .ToListAsync(cancellationToken);

            vendorsQuery = vendorsQuery.Where(v => vendorIdsWithCategory.Contains(v.Id));
        }

        var vendors = await vendorsQuery
            .Include(v => v.ServiceAreas.Where(sa => sa.IsActive))
            .OrderByDescending(v => v.AverageRating)
            .ThenByDescending(v => v.TotalReviews)
            .Take(50)
            .ToListAsync(cancellationToken);

        return vendors.Select(v => new VendorSearchResultDto(
            v.Id,
            v.BusinessName,
            v.BusinessDescription,
            v.BusinessLogo,
            v.AverageRating,
            v.TotalReviews,
            v.TotalProducts,
            v.ServiceAreas.Select(sa => sa.AreaName).ToList()
        )).ToList();
    }
}

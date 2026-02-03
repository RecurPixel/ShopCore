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
        var query = _context.VendorProfiles
            .AsNoTracking()
            .Include(v => v.ServiceAreas)
            .Include(v => v.Products)
            .Where(v => v.Status == VendorStatus.Active);

        // Filter by pincode if provided
        if (!string.IsNullOrEmpty(request.Pincode))
        {
            query = query.Where(v => v.ServiceAreas
                .Any(sa => sa.IsActive && sa.Pincodes.Contains(request.Pincode)));
        }

        // Filter by category if provided
        if (request.CategoryId.HasValue)
        {
            query = query.Where(v => v.Products
                .Any(p => p.CategoryId == request.CategoryId.Value && p.Status == ProductStatus.Active));
        }

        var vendors = await query
            .Select(v => new VendorSearchResultDto
            {
                Id = v.Id,
                BusinessName = v.BusinessName,
                BusinessDescription = v.BusinessDescription,
                BusinessLogo = v.BusinessLogo,
                AverageRating = v.AverageRating,
                TotalReviews = v.TotalReviews,
                TotalProducts = v.Products.Count(p => p.Status == ProductStatus.Active),
                RequiresDeposit = v.RequiresDeposit,
                DefaultDepositAmount = v.DefaultDepositAmount,
                ServiceAreas = v.ServiceAreas
                    .Where(sa => sa.IsActive)
                    .Select(sa => sa.AreaName)
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        // If latitude/longitude provided, calculate distance
        // (This is a simplified version - in production, use spatial queries)
        if (request.Latitude.HasValue && request.Longitude.HasValue)
        {
            // Sort by rating for now
            vendors = vendors.OrderByDescending(v => v.AverageRating).ToList();
        }
        else
        {
            vendors = vendors.OrderByDescending(v => v.AverageRating).ToList();
        }

        return vendors;
    }
}
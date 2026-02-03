using ShopCore.Application.Common.Models;
using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetAllVendors;

public class GetAllVendorsQueryHandler : IRequestHandler<GetAllVendorsQuery, PaginatedList<VendorProfileDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllVendorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<VendorProfileDto>> Handle(
        GetAllVendorsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.VendorProfiles
            .AsNoTracking()
            .Include(v => v.User)
            .AsQueryable();

        // Search filter
        if (!string.IsNullOrEmpty(request.Search))
        {
            var searchTerm = request.Search.ToLower();
            query = query.Where(v =>
                v.BusinessName.ToLower().Contains(searchTerm) ||
                v.User.Email.ToLower().Contains(searchTerm) ||
                v.User.PhoneNumber.Contains(searchTerm));
        }

        // Status filter
        if (!string.IsNullOrEmpty(request.Status)
            && Enum.TryParse<VendorStatus>(request.Status, out var status))
        {
            query = query.Where(v => v.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(v => v.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(v => new VendorProfileDto
            {
                Id = v.Id,
                UserId = v.UserId,
                BusinessName = v.BusinessName,
                BusinessLogo = v.BusinessLogo,
                Email = v.User.Email,
                PhoneNumber = v.User.PhoneNumber,
                Status = v.Status.ToString(),
                AverageRating = v.AverageRating,
                TotalReviews = v.TotalReviews,
                TotalProducts = v.TotalProducts,
                TotalOrders = v.TotalOrders,
                CreatedAt = v.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<VendorProfileDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize);
    }
}

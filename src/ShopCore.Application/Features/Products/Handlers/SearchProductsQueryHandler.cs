using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public SearchProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<ProductDto>> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .Include(p => p.Images)
            .Where(p => p.Status == ProductStatus.Active);

        // Search filter
        if (!string.IsNullOrEmpty(request.Query))
        {
            var searchTerm = request.Query.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                p.Description!.ToLower().Contains(searchTerm) ||
                p.Category.Name.ToLower().Contains(searchTerm) ||
                p.Vendor.BusinessName.ToLower().Contains(searchTerm));
        }

        // Category filter
        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.OrderCount)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                CompareAtPrice = p.CompareAtPrice,
                DiscountPercentage = p.DiscountPercentage,
                IsInStock = p.IsInStock,
                IsOnSale = p.IsOnSale,
                PrimaryImageUrl = p.Images.FirstOrDefault(i => i.IsPrimary) != null
                    ? p.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl
                    : null,
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                VendorId = p.VendorId,
                VendorName = p.Vendor != null ? p.Vendor.BusinessName : string.Empty
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<ProductDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}
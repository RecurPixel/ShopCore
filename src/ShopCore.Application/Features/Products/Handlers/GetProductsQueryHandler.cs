using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .Include(p => p.Images)
            .Where(p => p.Status == ProductStatus.Active);

        // Filters
        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);

        if (request.VendorId.HasValue)
            query = query.Where(p => p.VendorId == request.VendorId.Value);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice.Value);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice.Value);

        if (request.InStock.HasValue && request.InStock.Value)
            query = query.Where(p => !p.TrackInventory || p.StockQuantity > 0);

        // Sorting
        query = request.SortBy?.ToLower() switch
        {
            "price-asc" => query.OrderBy(p => p.Price),
            "price-desc" => query.OrderByDescending(p => p.Price),
            "rating" => query.OrderByDescending(p => p.AverageRating),
            "newest" => query.OrderByDescending(p => p.CreatedAt),
            "popular" => query.OrderByDescending(p => p.OrderCount),
            _ => query.OrderByDescending(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
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
                IsFeatured = p.IsFeatured,
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
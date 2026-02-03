using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetFeaturedProducts;

public class GetFeaturedProductsQueryHandler
    : IRequestHandler<GetFeaturedProductsQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetFeaturedProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(
        GetFeaturedProductsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .Include(p => p.Images)
            .Where(p => p.Status == ProductStatus.Active && p.IsFeatured)
            .OrderByDescending(p => p.AverageRating)
            .ThenByDescending(p => p.OrderCount)
            .Take(request.Limit)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                CompareAtPrice = p.CompareAtPrice,
                DiscountPercentage = p.DiscountPercentage,
                IsOnSale = p.IsOnSale,
                IsInStock = p.IsInStock,
                PrimaryImageUrl = p.Images.FirstOrDefault(i => i.IsPrimary) != null
                    ? p.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl
                    : null,
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                VendorId = p.VendorId,
                VendorName = p.Vendor.BusinessName
            })
            .ToListAsync(cancellationToken);
    }
}
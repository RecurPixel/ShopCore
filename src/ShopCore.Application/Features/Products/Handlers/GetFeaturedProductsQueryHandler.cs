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
                Description = p.Description,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                CompareAtPrice = p.CompareAtPrice,
                DiscountPercentage = p.DiscountPercentage,
                IsOnSale = p.IsOnSale,
                StockQuantity = p.StockQuantity,
                IsInStock = p.IsInStock,
                IsFeatured = p.IsFeatured,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                VendorId = p.VendorId,
                VendorName = p.Vendor.BusinessName,
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount,
                PrimaryImage = p.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl,
                Images = p.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList()
            })
            .ToListAsync(cancellationToken);
    }
}
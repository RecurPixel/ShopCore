using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Categories.Queries.GetProductsByCategory;

public class GetProductsByCategoryQueryHandler
    : IRequestHandler<GetProductsByCategoryQuery, List<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProductsByCategoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(
        GetProductsByCategoryQuery request,
        CancellationToken cancellationToken)
    {
        // Note: Assuming CategoryId is passed in the query
        // You may need to add it to the query record
        return await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .Include(p => p.Images)
            .Where(p => p.Status == ProductStatus.Active)
            // Add: && p.CategoryId == request.CategoryId
            .OrderByDescending(p => p.CreatedAt)
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
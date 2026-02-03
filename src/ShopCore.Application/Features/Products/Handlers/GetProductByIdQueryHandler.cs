using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDto> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .Include(p => p.Images)
            .Include(p => p.Specifications)
            .Where(p => p.Id == request.ProductId)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Description = p.Description,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                CompareAtPrice = p.CompareAtPrice,
                CostPerItem = p.CostPerItem,
                DiscountPercentage = p.DiscountPercentage,
                IsOnSale = p.IsOnSale,
                StockQuantity = p.StockQuantity,
                SKU = p.SKU,
                Barcode = p.Barcode,
                TrackInventory = p.TrackInventory,
                IsInStock = p.IsInStock,
                Weight = p.Weight,
                WeightUnit = p.WeightUnit,
                Dimensions = p.Dimensions,
                Status = p.Status.ToString(),
                IsFeatured = p.IsFeatured,
                IsSubscriptionAvailable = p.IsSubscriptionAvailable,
                SubscriptionDiscount = p.SubscriptionDiscount,
                MetaTitle = p.MetaTitle,
                MetaDescription = p.MetaDescription,
                MetaKeywords = p.MetaKeywords,
                ViewCount = p.ViewCount,
                OrderCount = p.OrderCount,
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                CategorySlug = p.Category.Slug,
                VendorId = p.VendorId,
                VendorName = p.Vendor.BusinessName,
                VendorRating = p.Vendor.AverageRating,
                PrimaryImage = p.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl,
                Images = p.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList(),
                Specifications = p.Specifications.Select(s => new ProductSpecificationDto
                {
                    Name = s.Name,
                    Value = s.Value
                }).ToList(),
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
            throw new NotFoundException(nameof(Product), request.ProductId);

        // Increment view count (async, don't await)
        _ = Task.Run(async () =>
        {
            var productEntity = await _context.Products.FindAsync(request.ProductId);
            if (productEntity != null)
            {
                productEntity.ViewCount++;
                await _context.SaveChangesAsync();
            }
        });

        return product;
    }
}

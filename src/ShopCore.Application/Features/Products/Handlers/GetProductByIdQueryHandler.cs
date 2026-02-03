using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailDto>
{
    private readonly IApplicationDbContext _context;

    public GetProductByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductDetailDto> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .Include(p => p.Images)
            .Include(p => p.Specifications)
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductDetailDto
            {
                Id = p.Id,
                Name = p.Name,
                Slug = p.Slug,
                Description = p.Description,
                ShortDescription = p.ShortDescription,
                Price = p.Price,
                CompareAtPrice = p.CompareAtPrice,
                DiscountPercentage = p.DiscountPercentage,
                StockQuantity = p.StockQuantity,
                SKU = p.SKU,
                IsInStock = p.IsInStock,
                IsOnSale = p.IsOnSale,
                IsFeatured = p.IsFeatured,
                Status = p.Status.ToString(),
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount,
                ViewCount = p.ViewCount,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                VendorName = p.Vendor != null ? p.Vendor.BusinessName : string.Empty,
                Images = p.Images.OrderBy(i => i.DisplayOrder).Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    IsPrimary = i.IsPrimary,
                    DisplayOrder = i.DisplayOrder
                }).ToList(),
                Specifications = p.Specifications.Select(s => new ProductSpecificationDto
                {
                    Name = s.Name,
                    Value = s.Value
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        // Increment view count
        await _context.Products
            .Where(p => p.Id == request.Id)
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.ViewCount, x => x.ViewCount + 1), cancellationToken);

        return product;
    }
}

using ShopCore.Application.Common.Models;
using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorProducts;

public class GetVendorProductsQueryHandler : IRequestHandler<GetVendorProductsQuery, PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;

    public GetVendorProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<ProductDto>> Handle(
        GetVendorProductsQuery request,
        CancellationToken cancellationToken)
    {
        if (!request.VendorId.HasValue)
            throw new ValidationException("VendorId is required");

        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.VendorId == request.VendorId.Value);

        // Public view - only active products
        if (request.publicOnly)
            query = query.Where(p => p.Status == ProductStatus.Active);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(p => p.CreatedAt)
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
                IsOnSale = p.IsOnSale,
                StockQuantity = p.StockQuantity,
                IsInStock = p.IsInStock,
                Status = p.Status.ToString(),
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                AverageRating = p.AverageRating,
                ReviewCount = p.ReviewCount,
                PrimaryImage = p.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<ProductDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize);
    }
}

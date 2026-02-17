using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        // 1. Find product with related data
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Vendor)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, ct);

        if (product == null)
            throw new NotFoundException("Product", request.Id);

        // 2. Verify ownership
        if (product.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only update your own products");

        // 3. Verify category if changed
        if (request.CategoryId != product.CategoryId)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId && !c.IsDeleted, ct);

            if (!categoryExists)
                throw new NotFoundException("Category", request.CategoryId);
        }

        // 4. Update fields (Status and IsFeatured are controlled via separate commands)
        product.Name = request.Name;
        product.Slug = request.Slug;
        product.Description = request.Description;
        product.ShortDescription = request.ShortDescription;
        product.Price = request.Price;
        product.CompareAtPrice = request.CompareAtPrice;
        product.CostPerItem = request.CostPerItem;
        product.StockQuantity = request.StockQuantity;
        product.SKU = request.SKU;
        product.Barcode = request.Barcode;
        product.TrackInventory = request.TrackInventory;
        product.Weight = request.Weight;
        product.WeightUnit = request.WeightUnit;
        product.Dimensions = request.Dimensions;
        // Note: Status is changed via UpdateProductStatusCommand
        // Note: IsFeatured is changed via FeatureProductCommand (admin only)
        product.IsSubscriptionAvailable = request.IsSubscriptionAvailable;
        product.SubscriptionDiscount = request.SubscriptionDiscount;
        product.MetaTitle = request.MetaTitle;
        product.MetaDescription = request.MetaDescription;
        product.MetaKeywords = request.MetaKeywords;
        product.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync(ct);

        // 5. Return updated ProductDto
        var primaryImage = product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                        ?? product.Images.FirstOrDefault()?.ImageUrl;

        var avgRating = product.Reviews.Any()
            ? product.Reviews.Average(r => r.Rating)
            : 0;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            ShortDescription = product.ShortDescription,
            Price = product.Price,
            CompareAtPrice = product.CompareAtPrice,
            DiscountPercentage = product.CompareAtPrice.HasValue && product.CompareAtPrice > 0
                ? Math.Round((product.CompareAtPrice.Value - product.Price) / product.CompareAtPrice.Value * 100, 0)
                : 0,
            IsInStock = !product.TrackInventory || product.StockQuantity > 0,
            IsOnSale = product.CompareAtPrice.HasValue && product.CompareAtPrice > product.Price,
            PrimaryImageUrl = primaryImage,
            AverageRating = (decimal)avgRating,
            ReviewCount = product.Reviews.Count,
            CategoryName = product.Category?.Name ?? string.Empty,
            VendorName = product.Vendor?.BusinessName ?? string.Empty
        };
    }
}

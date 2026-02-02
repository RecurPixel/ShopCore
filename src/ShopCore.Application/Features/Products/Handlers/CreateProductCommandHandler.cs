using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;

    public CreateProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken ct)
    {
        // 1. Verify vendor exists and is active
        var vendor = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.Id == _currentUser.VendorId, ct);

        if (vendor == null || vendor.Status != VendorStatus.Active)
            throw new ForbiddenException("Vendor account is not active");

        // 2. Verify category exists
        var categoryExists = await _context.Categories
            .AnyAsync(c => c.Id == request.CategoryId && !c.IsDeleted, ct);

        if (!categoryExists)
            throw new NotFoundException("Category", request.CategoryId);

        // 3. Generate unique slug
        var slug = await GenerateUniqueSlugAsync(request.Name, ct);

        // 4. Create product
        var product = new Product
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            Price = request.Price,
            CompareAtPrice = request.CompareAtPrice,
            CostPerItem = request.CostPerItem,
            StockQuantity = request.StockQuantity,
            SKU = request.SKU,
            Barcode = request.Barcode,
            TrackInventory = request.TrackInventory,
            Weight = request.Weight,
            WeightUnit = request.WeightUnit,
            Dimensions = request.Dimensions,
            Status = ProductStatus.Draft, // Starts as draft
            IsFeatured = false,
            IsSubscriptionAvailable = request.IsSubscriptionAvailable,
            SubscriptionDiscount = request.SubscriptionDiscount,
            MetaTitle = request.MetaTitle ?? request.Name,
            MetaDescription = request.MetaDescription ?? request.ShortDescription,
            MetaKeywords = request.MetaKeywords,
            VendorId = _currentUser.VendorId!.Value,
            CategoryId = request.CategoryId
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(ct);

        // 5. Increment vendor's product count
        vendor.TotalProducts++;
        await _context.SaveChangesAsync(ct);

        return MapToProductDto(product);
    }

    private async Task<string> GenerateUniqueSlugAsync(string name, CancellationToken ct)
    {
        var baseSlug = name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("'", "")
            .Replace("\"", "");

        var slug = baseSlug;
        var counter = 1;

        while (await _context.Products.AnyAsync(p => p.Slug == slug, ct))
        {
            slug = $"{baseSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    private ProductDto MapToProductDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Slug = p.Slug,
        Description = p.Description,
        Price = p.Price,
        CompareAtPrice = p.CompareAtPrice,
        StockQuantity = p.StockQuantity,
        Status = p.Status.ToString(),
        VendorId = p.VendorId,
        CategoryId = p.CategoryId
    };

}

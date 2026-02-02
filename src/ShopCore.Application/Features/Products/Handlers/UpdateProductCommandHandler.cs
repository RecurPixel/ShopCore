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

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        // 1. Find product
        var product = await _context.Products
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

        // 4. Update fields
        product.Name = request.Name;
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
        product.IsSubscriptionAvailable = request.IsSubscriptionAvailable;
        product.SubscriptionDiscount = request.SubscriptionDiscount;
        product.MetaTitle = request.MetaTitle;
        product.MetaDescription = request.MetaDescription;
        product.MetaKeywords = request.MetaKeywords;
        product.CategoryId = request.CategoryId;

        await _context.SaveChangesAsync(ct);

        return Unit.Value;
    }
}

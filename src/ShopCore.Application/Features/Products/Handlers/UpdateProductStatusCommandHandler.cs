using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.UpdateProductStatus;

public class UpdateProductStatusCommandHandler
    : IRequestHandler<UpdateProductStatusCommand, ProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateProductStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ProductDto> Handle(UpdateProductStatusCommand request, CancellationToken ct)
    {
        // 1. Find product
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, ct);

        if (product == null)
            throw new NotFoundException("Product", request.Id);

        // 2. Verify ownership
        if (product.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only update your own products");

        // 3. Update status
        product.Status = request.Status;

        // 4. If marking as active, verify required fields
        if (request.Status == ProductStatus.Active)
        {
            if (string.IsNullOrWhiteSpace(product.Description))
                throw new ValidationException("Description is required for active products");

            var hasImages = await _context.ProductImages
                .AnyAsync(pi => pi.ProductId == product.Id && !pi.IsDeleted, ct);

            if (!hasImages)
                throw new ValidationException("At least one image is required for active products");
        }

        await _context.SaveChangesAsync(ct);
    }
}

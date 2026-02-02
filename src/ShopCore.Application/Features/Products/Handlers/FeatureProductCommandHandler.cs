namespace ShopCore.Application.Products.Commands.FeatureProduct;

public class FeatureProductCommandHandler : IRequestHandler<FeatureProductCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public FeatureProductCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(FeatureProductCommand request, CancellationToken ct)
    {
        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null)
            throw new NotFoundException("Product", request.ProductId);

        // Vendor can feature their own products, admin can feature any
        if (product.VendorId != _currentUser.VendorId && _currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("You can only feature your own products");

        product.IsFeatured = request.IsFeatured;
        await _context.SaveChangesAsync(ct);
    }
}

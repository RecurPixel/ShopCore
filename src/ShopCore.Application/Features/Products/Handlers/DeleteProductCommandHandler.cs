namespace ShopCore.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteProductCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        // 1. Find product
        var product = await _context.Products
            .Include(p => p.CartItems)
            .Include(p => p.Wishlists)
            .FirstOrDefaultAsync(p => p.Id == request.Id && !p.IsDeleted, ct);

        if (product == null)
            throw new NotFoundException("Product", request.Id);

        // 2. Verify ownership
        if (product.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only delete your own products");

        // 3. Check if product is in active orders
        var hasActiveOrders = await _context.OrderItems
            .AnyAsync(oi => oi.ProductId == request.Id &&
                oi.Status != OrderStatus.Delivered &&
                oi.Status != OrderStatus.Cancelled, ct);

        if (hasActiveOrders)
            throw new ValidationException("Cannot delete product with active orders");

        // 4. Soft delete product
        product.IsDeleted = true;
        product.Status = ProductStatus.Discontinued;

        // 5. Remove from all carts
        _context.CartItems.RemoveRange(product.CartItems);

        // 6. Remove from all wishlists
        _context.Wishlists.RemoveRange(product.Wishlists);

        // 7. Decrement vendor's product count
        var vendor = await _context.VendorProfiles.FindAsync(_currentUser.VendorId);
        if (vendor != null)
            vendor.TotalProducts--;

        await _context.SaveChangesAsync(ct);

    }
}

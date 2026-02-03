using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateCartItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(UpdateCartItemCommand request, CancellationToken ct)
    {
        var cart = await _context.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Vendor)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, ct);

        if (cart == null)
            throw new NotFoundException("Cart not found");

        var cartItem = cart.Items.FirstOrDefaultAsync(i => i.ProductId == request.ProductId && !i.IsDeleted);
        if (cartItem == null)
            throw new NotFoundException("Product not in cart");

        var product = cartItem.Product;

        // Verify stock
        if (product.TrackInventory && product.StockQuantity < request.Quantity)
            throw new ValidationException($"Only {product.StockQuantity} units available");

        cartItem.Quantity = request.Quantity;
        cartItem.Price = product.Price; // Update to current price

        await _context.SaveChangesAsync(ct);

        return await BuildCartDtoAsync(cart, ct);
    }

    private async Task<CartDto> BuildCartDtoAsync(Domain.Entities.Cart cart, CancellationToken ct)
    {
        var items = cart.Items.Where(i => !i.IsDeleted).Select(i => new CartItemDto
        {
            Id = i.Id,
            ProductId = i.ProductId,
            ProductName = i.Product.Name,
            Price = i.Price,
            Quantity = i.Quantity,
            Subtotal = i.Quantity * i.Price,
            VendorId = i.Product.VendorId,
            VendorName = i.Product.Vendor.BusinessName,
            ImageUrl = i.Product.Images.FirstOrDefault(img => img.IsPrimary)?.ImageUrl
        }).ToList();

        var subtotal = items.Sum(i => i.Subtotal);
        var tax = subtotal * 0.18m;
        var total = subtotal + tax;

        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            Subtotal = subtotal,
            Tax = tax,
            Total = total,
            ItemCount = items.Sum(i => i.Quantity),
            AppliedCouponCode = cart.AppliedCouponCode,
            Discount = cart.Discount ?? 0
        };
    }
}

using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ITaxService _taxService;

    public UpdateCartItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, ITaxService taxService)
    {
        _context = context;
        _currentUser = currentUser;
        _taxService = taxService;
    }

    public async Task<CartDto> Handle(UpdateCartItemCommand request, CancellationToken ct)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Vendor)
            .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, ct);

        if (cart == null)
            throw new NotFoundException("Cart not found");

        var cartItem = await _context.CartItems
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == request.CartItemId && i.CartId == cart.Id && !i.IsDeleted, ct);

        if (cartItem == null)
            throw new NotFoundException("Cart item not found");

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
            ProductImageUrl = i.Product.Images.FirstOrDefault(img => img.IsPrimary)?.ImageUrl,
            Price = i.Price,
            Quantity = i.Quantity,
            Subtotal = i.Quantity * i.Price,
            IsInStock = !i.Product.TrackInventory || i.Product.StockQuantity >= i.Quantity,
            VendorId = i.Product.VendorId,
            VendorName = i.Product.Vendor.BusinessName
        }).ToList();

        var subtotal = items.Sum(i => i.Subtotal);
        var discount = cart.Discount ?? 0;
        var tax = _taxService.CalculateTax(subtotal, discount);

        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            Subtotal = subtotal,
            Discount = discount,
            Tax = tax,
            Total = subtotal + tax - discount,
            ItemCount = items.Sum(i => i.Quantity),
            AppliedCouponCode = cart.AppliedCouponCode
        };
    }
}

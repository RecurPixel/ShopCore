using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ITaxService _taxService;

    public RemoveCartItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, ITaxService taxService)
    {
        _context = context;
        _currentUser = currentUser;
        _taxService = taxService;
    }

    public async Task<CartDto> Handle(RemoveCartItemCommand request, CancellationToken ct)
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

        var cartItem = cart.Items.FirstOrDefault(i => i.Id == request.CartItemId && !i.IsDeleted);
        if (cartItem != null)
        {
            cartItem.IsDeleted = true;
            await _context.SaveChangesAsync(ct);
        }

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

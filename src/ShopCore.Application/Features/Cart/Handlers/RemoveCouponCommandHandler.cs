using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.RemoveCoupon;

public class RemoveCouponCommandHandler : IRequestHandler<RemoveCouponCommand, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ITaxService _taxService;

    public RemoveCouponCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, ITaxService taxService)
    {
        _context = context;
        _currentUser = currentUser;
        _taxService = taxService;
    }

    public async Task<CartDto> Handle(RemoveCouponCommand request, CancellationToken ct)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Vendor)
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, ct);

        if (cart == null)
        {
            return new CartDto
            {
                Items = new List<CartItemDto>(),
                Subtotal = 0,
                Discount = 0,
                Tax = 0,
                Total = 0,
                ItemCount = 0
            };
        }

        cart.AppliedCouponCode = null;
        cart.Discount = null;
        await _context.SaveChangesAsync(ct);

        var items = cart.Items
            .Where(ci => !ci.Product.IsDeleted && ci.Product.Status == ProductStatus.Active)
            .Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ProductImageUrl = ci.Product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                Price = ci.Product.Price,
                Quantity = ci.Quantity,
                Subtotal = ci.Product.Price * ci.Quantity,
                IsInStock = !ci.Product.TrackInventory || ci.Product.StockQuantity >= ci.Quantity,
                VendorId = ci.Product.VendorId,
                VendorName = ci.Product.Vendor.BusinessName
            })
            .ToList();

        var subtotal = items.Sum(i => i.Subtotal);
        var tax = _taxService.CalculateTax(subtotal, 0);

        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            Subtotal = subtotal,
            Discount = 0,
            Tax = tax,
            Total = subtotal + tax,
            ItemCount = items.Sum(i => i.Quantity)
        };
    }
}

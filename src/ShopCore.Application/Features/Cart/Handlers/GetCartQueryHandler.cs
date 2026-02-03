using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Queries.GetCart;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCartQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(
        GetCartQuery request,
        CancellationToken cancellationToken)
    {
        var cart = await _context.Carts
            .AsNoTracking()
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Vendor)
            .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                    .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, cancellationToken);

        if (cart == null)
        {
            return new CartDto
            {
                Items = new List<CartItemDto>(),
                Subtotal = 0,
                Discount = 0,
                Tax = 0,
                TotalAmount = 0,
                ItemCount = 0
            };
        }

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
        var discount = cart.Discount ?? 0;
        var tax = (subtotal - discount) * 0.18m; // 18% GST

        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            Subtotal = subtotal,
            Discount = discount,
            Tax = tax,
            TotalAmount = subtotal + tax - discount,
            ItemCount = items.Sum(i => i.Quantity),
            AppliedCouponCode = cart.AppliedCouponCode
        };
    }
}
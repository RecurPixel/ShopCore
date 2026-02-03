using ShopCore.Application.Cart.DTOs;
using CartEntity = ShopCore.Domain.Entities.Cart;

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
                Tax = 0,
                Total = 0,
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
                ProductSlug = ci.Product.Slug,
                ProductImage = ci.Product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
                VendorId = ci.Product.VendorId,
                VendorName = ci.Product.Vendor.BusinessName,
                Price = ci.Product.Price,
                Quantity = ci.Quantity,
                Subtotal = ci.Product.Price * ci.Quantity,
                IsInStock = !ci.Product.TrackInventory || ci.Product.StockQuantity >= ci.Quantity,
                MaxQuantity = ci.Product.TrackInventory ? ci.Product.StockQuantity : 999
            })
            .ToList();

        var subtotal = items.Sum(i => i.Subtotal);
        var tax = subtotal * 0.18m; // 18% GST

        return new CartDto
        {
            Items = items,
            Subtotal = subtotal,
            Tax = tax,
            Total = subtotal + tax,
            ItemCount = items.Sum(i => i.Quantity)
        };
    }
}
using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RemoveCartItemCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(RemoveCartItemCommand request, CancellationToken ct)
    {
        var cart = await _context.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Vendor)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, ct);

        if (cart == null)
            throw new NotFoundException("Cart not found");

        var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId && !i.IsDeleted);
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
            Price = i.Price,
            Quantity = i.Quantity,
            Subtotal = i.Quantity * i.Price
        }).ToList();

        return new CartDto
        {
            Id = cart.Id,
            Items = items,
            Subtotal = items.Sum(i => i.Subtotal),
            ItemCount = items.Sum(i => i.Quantity)
        };
    }
}

using ShopCore.Application.Cart.DTOs;
using CartEntity = ShopCore.Domain.Entities.Cart;

namespace ShopCore.Application.Cart.Commands.AddCartItem;

public class AddCartItemCommandHandler : IRequestHandler<AddCartItemCommand, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public AddCartItemCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<CartDto> Handle(AddCartItemCommand request, CancellationToken ct)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && !p.IsDeleted, ct);

        if (product == null)
            throw new NotFoundException("Product", request.ProductId);

        if (product.StockQuantity < request.Quantity)
            throw new BadRequestException("Insufficient stock");

        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, ct);

        if (cart == null)
        {
            cart = new CartEntity { UserId = _currentUser.UserId };
            _context.Carts.Add(cart);
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

        if (existingItem != null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Price = product.Price
            });
        }

        await _context.SaveChangesAsync(ct);

        return await _mediator.Send(new Cart.Queries.GetCart.GetCartQuery(), ct);
    }
}

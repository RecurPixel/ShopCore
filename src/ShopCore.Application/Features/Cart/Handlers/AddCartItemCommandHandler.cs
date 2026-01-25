using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.AddCartItem;

public class AddCartItemCommandHandler : IRequestHandler<AddCartItemCommand, CartDto>
{
    public Task<CartDto> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.FromResult(new CartDto());
    }
}

using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, CartDto>
{
    public Task<CartDto> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.FromResult(new CartDto());
    }
}

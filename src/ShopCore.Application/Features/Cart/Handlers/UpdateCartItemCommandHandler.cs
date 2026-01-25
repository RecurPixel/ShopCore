using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.UpdateCartItem;

public class UpdateCartItemCommandHandler : IRequestHandler<UpdateCartItemCommand, CartDto>
{
    public Task<CartDto> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.FromResult(new CartDto());
    }
}

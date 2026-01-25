using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.ValidateCart;

public class ValidateCartCommandHandler
    : IRequestHandler<ValidateCartCommand, CartValidationResultDto>
{
    public Task<CartValidationResultDto> Handle(
        ValidateCartCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new CartValidationResultDto());
    }
}

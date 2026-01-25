using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Queries.GetCart;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    public Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult(new CartDto());
    }
}

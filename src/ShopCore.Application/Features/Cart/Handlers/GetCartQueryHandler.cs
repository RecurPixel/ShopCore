namespace ShopCore.Application.Cart.Queries.GetCart;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, object>
{
    public Task<object> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}

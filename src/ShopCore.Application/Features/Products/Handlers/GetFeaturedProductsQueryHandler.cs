namespace ShopCore.Application.Products.Queries.GetFeaturedProducts;

public class GetFeaturedProductsQueryHandler : IRequestHandler<GetFeaturedProductsQuery, object>
{
    public Task<object> Handle(
        GetFeaturedProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}

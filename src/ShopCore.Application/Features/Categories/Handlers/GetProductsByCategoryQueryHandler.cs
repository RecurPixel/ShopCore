namespace ShopCore.Application.Categories.Queries.GetProductsByCategory;

public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, object>
{
    public Task<object> Handle(
        GetProductsByCategoryQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}

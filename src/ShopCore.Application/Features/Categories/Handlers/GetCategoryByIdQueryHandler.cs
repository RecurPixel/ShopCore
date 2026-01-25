namespace ShopCore.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, object>
{
    public Task<object> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}

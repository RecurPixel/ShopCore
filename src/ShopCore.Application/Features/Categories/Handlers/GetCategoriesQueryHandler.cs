using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public Task<List<CategoryDto>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new List<CategoryDto>());
    }
}

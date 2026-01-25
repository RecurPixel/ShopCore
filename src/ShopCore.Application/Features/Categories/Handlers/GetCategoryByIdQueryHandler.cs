using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public Task<CategoryDto> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new CategoryDto());
    }
}

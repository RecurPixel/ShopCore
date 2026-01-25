using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public Task<CategoryDto> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new CategoryDto());
    }
}

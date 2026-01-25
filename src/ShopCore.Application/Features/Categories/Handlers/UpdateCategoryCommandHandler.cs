using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
{
    public Task<CategoryDto> Handle(
        UpdateCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new CategoryDto());
    }
}

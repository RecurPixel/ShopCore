using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    int? ParentCategoryId,
    int DisplayOrder,
    bool IsActive
) : IRequest<CategoryDto>;

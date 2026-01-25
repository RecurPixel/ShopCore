using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name,
    string Slug,
    string? Description,
    string? ImageUrl,
    int? ParentCategoryId,
    int DisplayOrder,
    bool IsActive
) : IRequest<CategoryDto>;

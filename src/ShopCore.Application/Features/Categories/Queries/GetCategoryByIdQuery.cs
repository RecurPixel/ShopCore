using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery : IRequest<CategoryDto>;

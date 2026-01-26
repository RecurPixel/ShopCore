using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(uint Id) : IRequest<CategoryDto>;

using ShopCore.Application.Common.Models;
using ShopCore.Application.Categories.DTOs;

namespace ShopCore.Application.Categories.Queries.GetCategories;

public record GetCategoriesQuery(
    string? Search = null,
    int? ParentId = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<CategoryDto>>;

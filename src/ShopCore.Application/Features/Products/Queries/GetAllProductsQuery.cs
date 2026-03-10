using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(
    string? Search = null,
    int? CategoryId = null,
    int? VendorId = null,
    bool? IsFeatured = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<ProductDto>>;

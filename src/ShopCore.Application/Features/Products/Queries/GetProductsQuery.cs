using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetProducts;

public record GetProductsQuery(
    int? CategoryId = null,
    int? VendorId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? InStock = null,
    string? SortBy = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<ProductDto>>;

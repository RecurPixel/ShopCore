using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetFeaturedProducts;

public record GetFeaturedProductsQuery(
    int? CategoryId = null,
    int? VendorId = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<ProductDto>>;

using ShopCore.Application.Common.Models;
using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorProducts;

public record GetVendorProductsQuery(
    int? VendorId,
    int Page = 1,
    int PageSize = 20,
    bool publicOnly = true
) : IRequest<PaginatedList<ProductDto>>;

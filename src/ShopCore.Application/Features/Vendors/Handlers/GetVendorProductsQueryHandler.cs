using ShopCore.Application.Common.Models;
using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorProducts;

public class GetVendorProductsQueryHandler : IRequestHandler<GetVendorProductsQuery, PaginatedList<ProductDto>>
{
    public Task<PaginatedList<ProductDto>> Handle(
        GetVendorProductsQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}

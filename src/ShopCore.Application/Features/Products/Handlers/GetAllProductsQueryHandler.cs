using ShopCore.Application.Common.Models;
using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginatedList<ProductDto>>
{
    public Task<PaginatedList<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

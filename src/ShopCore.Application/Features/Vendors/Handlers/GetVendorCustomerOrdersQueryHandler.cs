using ShopCore.Application.Common.Models;
using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerOrders;

public class GetVendorCustomerOrdersQueryHandler : IRequestHandler<GetVendorCustomerOrdersQuery, PaginatedList<VendorOrderDto>>
{
    public Task<PaginatedList<VendorOrderDto>> Handle(
        GetVendorCustomerOrdersQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}

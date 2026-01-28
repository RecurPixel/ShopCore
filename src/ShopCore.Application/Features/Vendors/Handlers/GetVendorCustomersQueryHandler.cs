using ShopCore.Application.Common.Models;
using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomers;

public class GetVendorCustomersQueryHandler : IRequestHandler<GetVendorCustomersQuery, PaginatedList<VendorCustomerDto>>
{
    public Task<PaginatedList<VendorCustomerDto>> Handle(
        GetVendorCustomersQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}

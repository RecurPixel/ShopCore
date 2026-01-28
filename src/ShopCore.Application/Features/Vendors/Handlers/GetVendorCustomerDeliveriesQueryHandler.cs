using ShopCore.Application.Common.Models;
using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerDeliveries;

public class GetVendorCustomerDeliveriesQueryHandler : IRequestHandler<GetVendorCustomerDeliveriesQuery, PaginatedList<DeliveryDto>>
{
    public Task<PaginatedList<DeliveryDto>> Handle(
        GetVendorCustomerDeliveriesQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}

using ShopCore.Application.Common.Models;
using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Queries.GetVendorDeliveries;

public class GetVendorDeliveriesQueryHandler : IRequestHandler<GetVendorDeliveriesQuery, PaginatedList<DeliveryDto>>
{
    public Task<PaginatedList<DeliveryDto>> Handle(
        GetVendorDeliveriesQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}

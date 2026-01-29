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
        // 1. Get current vendor from context
        // 2. Find customer by user id
        // 3. Get deliveries for subscriptions from this vendor
        // 4. Apply pagination (request.Page, request.PageSize)
        // 5. Include delivery items, status, tracking info
        // 6. Sort by scheduled date
        // 7. Map to DeliveryDto list
        // 8. Return PaginatedList<DeliveryDto>
        return Task.FromResult(new PaginatedList<DeliveryDto>([], 0, request.Page, request.PageSize));
    }
}

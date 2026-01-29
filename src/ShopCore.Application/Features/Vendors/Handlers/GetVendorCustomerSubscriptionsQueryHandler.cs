using ShopCore.Application.Common.Models;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerSubscriptions;

public class GetVendorCustomerSubscriptionsQueryHandler : IRequestHandler<GetVendorCustomerSubscriptionsQuery, PaginatedList<VendorSubscriptionDto>>
{
    public Task<PaginatedList<VendorSubscriptionDto>> Handle(
        GetVendorCustomerSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Get current vendor from context
        // 2. Find customer by user id
        // 3. Get subscriptions for this customer that contain vendor's products
        // 4. Apply pagination (request.Page, request.PageSize)
        // 5. Include subscription items, delivery schedule, status
        // 6. Map to VendorSubscriptionDto list
        // 7. Return PaginatedList<VendorSubscriptionDto>
        return Task.FromResult(new PaginatedList<VendorSubscriptionDto>([], 0, request.Page, request.PageSize));
    }
}

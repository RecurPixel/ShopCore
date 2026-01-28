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
        throw new NotImplementedException();
    }
}

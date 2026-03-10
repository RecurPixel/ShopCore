using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerSubscriptions;

public record GetVendorCustomerSubscriptionsQuery(
    int UserId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<VendorSubscriptionDto>>;

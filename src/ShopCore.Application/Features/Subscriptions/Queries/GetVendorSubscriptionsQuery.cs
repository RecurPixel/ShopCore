using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptions;

public record GetVendorSubscriptionsQuery(
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<VendorSubscriptionDto>>;

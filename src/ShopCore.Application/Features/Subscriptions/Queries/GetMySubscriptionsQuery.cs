using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetMySubscriptions;

public record GetMySubscriptionsQuery(
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<SubscriptionDto>>;

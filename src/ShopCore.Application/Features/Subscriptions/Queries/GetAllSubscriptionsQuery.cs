using ShopCore.Application.Common.Models;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetAllSubscriptions;

public record GetAllSubscriptionsQuery(
    string? Status = null,
    int? UserId = null,
    int? VendorId = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<SubscriptionDto>>;

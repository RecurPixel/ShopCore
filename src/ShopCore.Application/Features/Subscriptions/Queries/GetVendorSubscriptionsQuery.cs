using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptions;

public record GetVendorSubscriptionsQuery : IRequest<List<SubscriptionDto>>;

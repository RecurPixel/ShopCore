using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetMySubscriptions;

public record GetMySubscriptionsQuery : IRequest<List<SubscriptionDto>>;

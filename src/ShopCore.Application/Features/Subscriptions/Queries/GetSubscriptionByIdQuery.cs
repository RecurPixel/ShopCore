using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetSubscriptionById;

public record GetSubscriptionByIdQuery : IRequest<SubscriptionDto>;

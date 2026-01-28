using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetSubscriptionCustomerInfo;

public record GetSubscriptionCustomerInfoQuery(int SubscriptionId) : IRequest<SubscriptionCustomerInfoDto?>;

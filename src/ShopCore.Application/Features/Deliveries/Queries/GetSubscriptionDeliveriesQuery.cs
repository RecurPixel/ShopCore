using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Queries.GetSubscriptionDeliveries;

public record GetSubscriptionDeliveriesQuery(
    int SubscriptionId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<DeliveryDto>>;

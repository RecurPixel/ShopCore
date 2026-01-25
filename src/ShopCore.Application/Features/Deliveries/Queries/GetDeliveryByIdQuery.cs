using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Queries.GetDeliveryById;

public record GetDeliveryByIdQuery : IRequest<DeliveryDto>;

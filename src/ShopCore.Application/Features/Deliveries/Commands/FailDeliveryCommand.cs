using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Commands.FailDelivery;

public record FailDeliveryCommand(int Id, string Reason) : IRequest<DeliveryDto>;

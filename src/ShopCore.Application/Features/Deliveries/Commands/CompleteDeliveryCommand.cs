using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Commands.CompleteDelivery;

public record CompleteDeliveryCommand(
    int Id,
    PaymentMethod? PaymentMethod,
    string? PaymentTransactionId
) : IRequest<DeliveryDto>;

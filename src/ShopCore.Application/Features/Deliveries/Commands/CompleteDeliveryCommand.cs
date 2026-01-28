using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Commands.CompleteDelivery;

public record CompleteDeliveryCommand(
    int Id,
    List<DeliveryItemStatusInput>? ItemStatuses,
    PaymentMethod? PaymentMethod,
    string? PaymentTransactionId,
    string? DeliveryPhotoUrl,
    string? CustomerSignatureUrl,
    string? DeliveryNotes
) : IRequest<DeliveryDto>;

public record DeliveryItemStatusInput(
    int DeliveryItemId,
    DeliveryItemStatus Status,
    string? Notes
);

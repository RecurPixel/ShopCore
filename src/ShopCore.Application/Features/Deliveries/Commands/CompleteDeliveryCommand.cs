using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Commands.CompleteDelivery;

// Note: PaymentMethod is determined by the delivery record, not user input
// For COD deliveries, completing the delivery marks payment as received
// For prepaid deliveries, payment is handled separately via payment gateway
public record CompleteDeliveryCommand(
    int Id,
    List<DeliveryItemStatusInput>? ItemStatuses,
    string? CollectedPaymentReference,  // For COD: reference number for cash/payment collected
    string? DeliveryPhotoUrl,
    string? CustomerSignatureUrl,
    string? DeliveryNotes
) : IRequest<DeliveryDto>;

public record DeliveryItemStatusInput(
    int DeliveryItemId,
    DeliveryItemStatus Status,
    string? Notes
);

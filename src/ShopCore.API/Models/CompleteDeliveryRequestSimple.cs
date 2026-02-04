using ShopCore.Application.Deliveries.Commands.CompleteDelivery;
using ShopCore.Domain.Enums;

namespace ShopCore.Api.Models;

/// <summary>
/// Simpler URL-only version of CompleteDeliveryRequest.
/// Use POST /deliveries/{id}/photo to upload files first, then complete with URLs.
/// </summary>
public class CompleteDeliveryRequestSimple
{
    public List<DeliveryItemStatusRequest>? ItemStatuses { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? PaymentTransactionId { get; set; }
    public string? DeliveryPhotoUrl { get; set; }
    public string? CustomerSignatureUrl { get; set; }
    public string? DeliveryNotes { get; set; }
}

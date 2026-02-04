using ShopCore.Domain.Enums;

namespace ShopCore.Api.Models;

public class CompleteDeliveryRequest
{
    public IFormFile? DeliveryPhoto { get; set; }
    public IFormFile? CustomerSignature { get; set; }
    public List<DeliveryItemStatusRequest>? ItemStatuses { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? PaymentTransactionId { get; set; }
    public string? DeliveryPhotoUrl { get; set; }
    public string? CustomerSignatureUrl { get; set; }
    public string? DeliveryNotes { get; set; }
}

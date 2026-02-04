namespace ShopCore.Application.Deliveries.DTOs;

public record DeliveryDto
{
    public int Id { get; init; }
    public string DeliveryNumber { get; init; } = string.Empty;
    public int SubscriptionId { get; init; }
    public string? SubscriptionNumber { get; init; }

    // Customer Information
    public string? CustomerName { get; init; }
    public string? CustomerPhone { get; init; }

    // Delivery Address
    public string? DeliveryAddress { get; init; }
    public string? DeliveryCity { get; init; }
    public string? DeliveryState { get; init; }
    public string? Pincode { get; init; }
    public string? Landmark { get; init; }

    // Delivery Details
    public DateTime ScheduledDate { get; init; }
    public DateTime? ActualDeliveryDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? DeliveryPersonName { get; init; }
    public string? DeliveryNotes { get; init; }
    public string? FailureReason { get; init; }
    public string? SkipReason { get; init; }

    // Proof of Delivery
    public string? DeliveryPhotoUrl { get; init; }
    public string? CustomerSignatureUrl { get; init; }

    // Payment Information
    public string? PaymentStatus { get; init; }
    public decimal Total { get; init; }
    public string? PaymentMethod { get; init; }
    public string? PaymentGateway { get; init; }
    public string? PaymentTransactionId { get; init; }
    public DateTime? PaidAt { get; init; }

    // Invoice
    public int? InvoiceId { get; init; }
    public string? InvoiceNumber { get; init; }

    // Items
    public int ItemCount { get; init; }
    public List<DeliveryItemDto> Items { get; init; } = new();

    // Metadata
    public DateTime CreatedAt { get; init; }
}

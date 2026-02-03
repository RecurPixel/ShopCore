namespace ShopCore.Application.Deliveries.DTOs;

public record DeliveryDto
{
    public int Id { get; init; }
    public string DeliveryNumber { get; init; } = string.Empty;
    public int SubscriptionId { get; init; }
    public string? SubscriptionNumber { get; init; }
    public DateTime ScheduledDate { get; init; }
    public DateTime? ActualDeliveryDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? PaymentStatus { get; init; }
    public decimal TotalAmount { get; init; }
    public string? PaymentMethod { get; init; }
    public DateTime? PaidAt { get; init; }
    public string? DeliveryPersonName { get; init; }
    public string? FailureReason { get; init; }
    public string? SkipReason { get; init; }
    public int? InvoiceId { get; init; }
    public string? InvoiceNumber { get; init; }
    public int ItemCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public List<DeliveryItemDto> Items { get; init; } = new();
}

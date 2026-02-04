namespace ShopCore.Application.Invoices.DTOs;

public record InvoiceDeliveryDto
{
    public int DeliveryId { get; init; }
    public string DeliveryNumber { get; init; } = string.Empty;
    public DateTime ScheduledDate { get; init; }
    public DateTime? ActualDeliveryDate { get; init; }
    public decimal TotalAmount { get; init; }
    public int ItemCount { get; init; }
}

namespace ShopCore.Application.Invoices.DTOs;

public record InvoiceDto
{
    // Core Properties
    public int Id { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public int SubscriptionId { get; init; }
    public string SubscriptionNumber { get; init; } = string.Empty;

    // Dates
    public DateTime InvoiceDate { get; init; }
    public DateTime GeneratedAt { get; init; }
    public DateTime DueDate { get; init; }
    public DateTime? PaidAt { get; init; }
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public DateTime CreatedAt { get; init; }

    // Amounts
    public decimal Subtotal { get; init; }
    public decimal Tax { get; init; }
    public decimal Total { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal BalanceDue { get; init; }

    // Status
    public string Status { get; init; } = string.Empty;
    public InvoiceStatus InvoiceStatus { get; init; }
    public bool IsManuallyGenerated { get; init; }
    public bool IsOverdue { get; init; }
    public bool IsFullyPaid { get; init; }

    // Payment Info
    public string? PaymentMethod { get; init; }
    public PaymentMethod? PaymentMethodEnum { get; init; }
    public string? PaymentTransactionId { get; init; }

    // Customer Info
    public int UserId { get; init; }
    public string? CustomerName { get; init; }
    public string? CustomerEmail { get; init; }
    public string? CustomerPhone { get; init; }

    // Vendor Info
    public int VendorId { get; init; }
    public string? VendorName { get; init; }
    public string? VendorAddress { get; init; }
    public string? VendorGst { get; init; }

    // Details
    public int TotalDeliveries { get; init; }
    public List<InvoiceLineItemDto> LineItems { get; init; } = new();
    public List<InvoiceDeliveryDto>? Deliveries { get; init; }
}

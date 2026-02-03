namespace ShopCore.Application.Invoices.DTOs;

public record InvoiceDto
{
    public int Id { get; init; }
    public string InvoiceNumber { get; init; } = string.Empty;
    public int SubscriptionId { get; init; }
    public string SubscriptionNumber { get; init; } = string.Empty;
    public DateTime InvoiceDate { get; init; }
    public DateTime DueDate { get; init; }
    public decimal SubTotal { get; init; }
    public decimal Tax { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal BalanceDue { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? PaidAt { get; init; }
    public string? PaymentMethod { get; init; }
    public string? PaymentTransactionId { get; init; }
    public List<InvoiceLineItemDto> LineItems { get; init; } = new();
}

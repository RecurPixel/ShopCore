namespace ShopCore.Application.Invoices.DTOs;

public record InvoiceDto(
    int Id,
    string InvoiceNumber,
    int SubscriptionId,
    string SubscriptionNumber,
    DateTime InvoiceDate,
    DateTime DueDate,
    decimal SubTotal,
    decimal Tax,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal BalanceDue,
    InvoiceStatus Status,
    DateTime? PaidAt,
    PaymentMethod? PaymentMethod,
    string? PaymentTransactionId,
    List<InvoiceLineItemDto> LineItems
);

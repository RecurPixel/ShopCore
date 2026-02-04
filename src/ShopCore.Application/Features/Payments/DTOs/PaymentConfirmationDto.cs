namespace ShopCore.Application.Payments.DTOs;

public record PaymentConfirmationDto
{
    public int ReferenceId { get; init; }  // Order or Invoice ID
    public string ReferenceType { get; init; } = string.Empty;  // "Order" or "Invoice"
    public string? ReferenceNumber { get; init; }  // OrderNumber or InvoiceNumber
    public string PaymentId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime? ConfirmedAt { get; init; }
}

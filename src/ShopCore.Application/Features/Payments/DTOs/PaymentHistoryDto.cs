namespace ShopCore.Application.Payments.DTOs;

public record PaymentHistoryDto(
    int Id,
    string TransactionId,
    decimal Amount,
    string Currency,
    PaymentMethod PaymentMethod,
    PaymentStatus Status,
    string? OrderNumber,
    string? InvoiceNumber,
    string? Description,
    DateTime CreatedAt,
    DateTime? CompletedAt
);

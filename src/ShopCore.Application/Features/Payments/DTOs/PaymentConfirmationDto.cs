namespace ShopCore.Application.Payments.DTOs;

public record PaymentConfirmationDto(
    string PaymentIntentId,
    string TransactionId,
    PaymentStatus Status,
    decimal Amount,
    DateTime ConfirmedAt
);

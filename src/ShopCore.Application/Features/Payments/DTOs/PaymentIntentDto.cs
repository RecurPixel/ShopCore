namespace ShopCore.Application.Payments.DTOs;

public record PaymentIntentDto(
    string PaymentIntentId,
    string ClientSecret,
    decimal Amount,
    string Currency,
    PaymentStatus Status
);

using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.ConfirmPayment;

public record ConfirmPaymentCommand(string PaymentIntentId, string TransactionId)
    : IRequest<PaymentConfirmationDto>;

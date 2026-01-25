using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.CreatePaymentIntent;

public record CreatePaymentIntentCommand(
    int? OrderId,
    int? InvoiceId,
    decimal Amount,
    string Currency
) : IRequest<PaymentIntentDto>;

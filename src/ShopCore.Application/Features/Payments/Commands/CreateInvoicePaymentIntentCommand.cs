using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.CreateInvoicePaymentIntent;

public record CreateInvoicePaymentIntentCommand(int InvoiceId) : IRequest<PaymentIntentDto>;

using ShopCore.Application.Payments.DTOs;
using ShopCore.Domain.Enums;

namespace ShopCore.Application.Payments.Commands.CreateInvoicePaymentIntent;

/// <summary>
/// Command to create a payment intent for a subscription invoice
/// </summary>
/// <param name="InvoiceId">The invoice ID to create payment for</param>
/// <param name="Gateway">Optional payment gateway to use. If null, uses default gateway.</param>
public record CreateInvoicePaymentIntentCommand(
    int InvoiceId,
    PaymentGateway? Gateway = null
) : IRequest<PaymentIntentDto>;

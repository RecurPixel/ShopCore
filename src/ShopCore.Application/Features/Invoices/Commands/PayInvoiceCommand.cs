using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Commands.PayInvoice;

public record PayInvoiceCommand(
    int InvoiceId,
    PaymentMethod PaymentMethod,
    string? PaymentTransactionId
) : IRequest<InvoiceDto>;

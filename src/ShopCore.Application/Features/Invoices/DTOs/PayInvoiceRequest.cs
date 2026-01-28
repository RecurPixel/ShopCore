namespace ShopCore.Application.Invoices.DTOs;

public record PayInvoiceRequest(
    PaymentMethod PaymentMethod,
    string? PaymentTransactionId
);

using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Commands.GenerateSubscriptionInvoice;

public record GenerateSubscriptionInvoiceCommand(int SubscriptionId) : IRequest<InvoiceDto>;

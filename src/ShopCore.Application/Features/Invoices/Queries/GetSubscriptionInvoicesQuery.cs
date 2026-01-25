using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.GetSubscriptionInvoices;

public record GetSubscriptionInvoicesQuery : IRequest<List<InvoiceDto>>;

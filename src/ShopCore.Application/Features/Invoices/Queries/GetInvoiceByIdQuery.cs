using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.GetInvoiceById;

public record GetInvoiceByIdQuery(int InvoiceId) : IRequest<InvoiceDto>;

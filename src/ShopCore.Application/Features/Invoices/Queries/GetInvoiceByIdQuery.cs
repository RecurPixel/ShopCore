using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.GetInvoiceById;

public record GetInvoiceByIdQuery : IRequest<InvoiceDto>;

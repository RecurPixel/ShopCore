using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.DownloadInvoice;

public record DownloadInvoiceQuery(int Id) : IRequest<InvoiceDownloadDto>;

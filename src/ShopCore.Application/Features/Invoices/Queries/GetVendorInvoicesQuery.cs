using ShopCore.Application.Common.Models;
using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.GetVendorInvoices;

public record GetVendorInvoicesQuery(
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<InvoiceDto>>;

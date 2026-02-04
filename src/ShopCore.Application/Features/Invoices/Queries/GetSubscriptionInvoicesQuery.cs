using ShopCore.Application.Common.Models;
using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.GetSubscriptionInvoices;

public record GetSubscriptionInvoicesQuery(
    int SubscriptionId,
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<InvoiceDto>>;

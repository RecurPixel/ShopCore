using ShopCore.Application.Common.Models;
using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.GetVendorInvoices;

public class GetVendorInvoicesQueryHandler : IRequestHandler<GetVendorInvoicesQuery, PaginatedList<InvoiceDto>>
{
    public Task<PaginatedList<InvoiceDto>> Handle(
        GetVendorInvoicesQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}

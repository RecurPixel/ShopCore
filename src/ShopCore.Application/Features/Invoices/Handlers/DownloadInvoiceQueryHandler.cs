using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.DownloadInvoice;

public class DownloadInvoiceQueryHandler : IRequestHandler<DownloadInvoiceQuery, InvoiceDownloadDto>
{
    public Task<InvoiceDownloadDto> Handle(
        DownloadInvoiceQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        // 1. Get invoice by id
        // 2. Verify user has access (owner or vendor)
        // 3. Generate or retrieve PDF
        // 4. Return InvoiceDownloadDto with file content
        return Task.FromResult(new InvoiceDownloadDto());
    }
}

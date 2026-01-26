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
        return Task.FromResult(new InvoiceDownloadDto());
    }
}

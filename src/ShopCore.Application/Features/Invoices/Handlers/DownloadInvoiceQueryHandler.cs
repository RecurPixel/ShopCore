namespace ShopCore.Application.Invoices.Queries.DownloadInvoice;

public class DownloadInvoiceQueryHandler : IRequestHandler<DownloadInvoiceQuery, object>
{
    public Task<object> Handle(DownloadInvoiceQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}

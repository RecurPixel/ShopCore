using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.GetSubscriptionInvoices;

public class GetSubscriptionInvoicesQueryHandler
    : IRequestHandler<GetSubscriptionInvoicesQuery, List<InvoiceDto>>
{
    public Task<List<InvoiceDto>> Handle(
        GetSubscriptionInvoicesQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}

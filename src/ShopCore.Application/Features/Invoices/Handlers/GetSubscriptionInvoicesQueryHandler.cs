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
        // TODO: Implement query logic        // 1. Get subscription by id
        // 2. Verify user owns this subscription (or vendor)
        // 3. Fetch all invoices for this subscription
        // 4. Filter by status if provided (pending, paid, overdue, cancelled)
        // 5. Filter by date range if provided
        // 6. Include payment details and transaction info
        // 7. Sort by invoice date and apply pagination
        // 8. Map to InvoiceDto list and return        return Task.FromResult<object>(new { });
    }
}

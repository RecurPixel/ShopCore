namespace ShopCore.Application.Invoices.Queries.GetSubscriptionInvoices;

public class GetSubscriptionInvoicesQueryHandler
    : IRequestHandler<GetSubscriptionInvoicesQuery, object>
{
    public Task<object> Handle(
        GetSubscriptionInvoicesQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}

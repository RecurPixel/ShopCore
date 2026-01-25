namespace ShopCore.Application.Orders.Queries.GetOrderInvoice;

public class GetOrderInvoiceQueryHandler : IRequestHandler<GetOrderInvoiceQuery, object>
{
    public Task<object> Handle(GetOrderInvoiceQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}

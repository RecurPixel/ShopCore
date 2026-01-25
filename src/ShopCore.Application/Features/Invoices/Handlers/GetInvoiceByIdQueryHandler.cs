namespace ShopCore.Application.Invoices.Queries.GetInvoiceById;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, object>
{
    public Task<object> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}

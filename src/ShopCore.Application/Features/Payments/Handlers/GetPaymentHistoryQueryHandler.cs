namespace ShopCore.Application.Payments.Queries.GetPaymentHistory;

public class GetPaymentHistoryQueryHandler : IRequestHandler<GetPaymentHistoryQuery, object>
{
    public Task<object> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}

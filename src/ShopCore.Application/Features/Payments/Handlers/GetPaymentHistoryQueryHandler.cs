using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Queries.GetPaymentHistory;

public class GetPaymentHistoryQueryHandler
    : IRequestHandler<GetPaymentHistoryQuery, List<PaymentHistoryDto>>
{
    public Task<List<PaymentHistoryDto>> Handle(
        GetPaymentHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new List<PaymentHistoryDto>());
    }
}

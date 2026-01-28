using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetSubscriptionCustomerInfo;

public class GetSubscriptionCustomerInfoQueryHandler : IRequestHandler<GetSubscriptionCustomerInfoQuery, SubscriptionCustomerInfoDto?>
{
    public Task<SubscriptionCustomerInfoDto?> Handle(
        GetSubscriptionCustomerInfoQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}

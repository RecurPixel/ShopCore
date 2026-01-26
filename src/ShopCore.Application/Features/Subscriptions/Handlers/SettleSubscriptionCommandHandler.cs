using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.SettleSubscription;

public class SettleSubscriptionCommandHandler
    : IRequestHandler<SettleSubscriptionCommand, SubscriptionSettlementDto>
{
    public Task<SubscriptionSettlementDto> Handle(
        SettleSubscriptionCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new SubscriptionSettlementDto());
    }
}

namespace ShopCore.Application.Subscriptions.Commands.SettleSubscription;

public class SettleSubscriptionCommandHandler : IRequestHandler<SettleSubscriptionCommand>
{
    public Task Handle(SettleSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}

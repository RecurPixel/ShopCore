namespace ShopCore.Application.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand>
{
    public Task Handle(
        CancelSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        throw new NotImplementedException();
    }
}

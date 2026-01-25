namespace ShopCore.Application.Subscriptions.Commands.PauseSubscription;

public class PauseSubscriptionCommandHandler : IRequestHandler<PauseSubscriptionCommand>
{
    public Task Handle(PauseSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}

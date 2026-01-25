namespace ShopCore.Application.Subscriptions.Commands.ResumeSubscription;

public class ResumeSubscriptionCommandHandler : IRequestHandler<ResumeSubscriptionCommand>
{
    public Task Handle(ResumeSubscriptionCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}

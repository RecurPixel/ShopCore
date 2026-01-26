using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.ResumeSubscription;

public class ResumeSubscriptionCommandHandler
    : IRequestHandler<ResumeSubscriptionCommand, SubscriptionDto>
{
    public Task<SubscriptionDto> Handle(
        ResumeSubscriptionCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new SubscriptionDto());
    }
}

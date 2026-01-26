using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.PauseSubscription;

public class PauseSubscriptionCommandHandler
    : IRequestHandler<PauseSubscriptionCommand, SubscriptionDto>
{
    public Task<SubscriptionDto> Handle(
        PauseSubscriptionCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new SubscriptionDto());
    }
}

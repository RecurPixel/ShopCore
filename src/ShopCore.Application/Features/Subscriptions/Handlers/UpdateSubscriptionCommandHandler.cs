using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.UpdateSubscription;

public class UpdateSubscriptionCommandHandler
    : IRequestHandler<UpdateSubscriptionCommand, SubscriptionDto>
{
    public Task<SubscriptionDto> Handle(
        UpdateSubscriptionCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new SubscriptionDto());
    }
}

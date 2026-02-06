using ShopCore.Application.Subscriptions.Commands.PauseSubscription;

namespace ShopCore.Application.Subscriptions.Validators;

public class PauseSubscriptionCommandValidator : AbstractValidator<PauseSubscriptionCommand>
{
    public PauseSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .GreaterThan(0)
            .WithMessage("Subscription ID is required");
    }
}

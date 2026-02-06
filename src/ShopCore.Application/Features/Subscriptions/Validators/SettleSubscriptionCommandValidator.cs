using ShopCore.Application.Subscriptions.Commands.SettleSubscription;

namespace ShopCore.Application.Subscriptions.Validators;

public class SettleSubscriptionCommandValidator : AbstractValidator<SettleSubscriptionCommand>
{
    public SettleSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .GreaterThan(0)
            .WithMessage("Subscription ID is required");
    }
}

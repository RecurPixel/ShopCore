using ShopCore.Application.Subscriptions.Commands.CancelSubscription;

namespace ShopCore.Application.Subscriptions.Validators;

public class CancelSubscriptionCommandValidator : AbstractValidator<CancelSubscriptionCommand>
{
    public CancelSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .GreaterThan(0)
            .WithMessage("Subscription ID is required");
    }
}

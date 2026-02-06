using ShopCore.Application.Subscriptions.Commands.ResumeSubscription;

namespace ShopCore.Application.Subscriptions.Validators;

public class ResumeSubscriptionCommandValidator : AbstractValidator<ResumeSubscriptionCommand>
{
    public ResumeSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .GreaterThan(0)
            .WithMessage("Subscription ID is required");
    }
}

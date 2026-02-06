using ShopCore.Application.Subscriptions.Commands.ConvertToSubscription;

namespace ShopCore.Application.Subscriptions.Validators;

public class ConvertToSubscriptionCommandValidator : AbstractValidator<ConvertToSubscriptionCommand>
{
    public ConvertToSubscriptionCommandValidator()
    {
        RuleFor(x => x.OneTimeSubscriptionId)
            .GreaterThan(0)
            .WithMessage("One-time subscription ID is required");

        RuleFor(x => x.Frequency)
            .IsInEnum()
            .WithMessage("Invalid subscription frequency");

        RuleFor(x => x.BillingCycleDays)
            .GreaterThan(0)
            .WithMessage("Billing cycle days must be greater than 0");
    }
}

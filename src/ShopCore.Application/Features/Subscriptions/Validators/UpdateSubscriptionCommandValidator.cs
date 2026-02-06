using ShopCore.Application.Subscriptions.Commands.UpdateSubscription;

namespace ShopCore.Application.Subscriptions.Validators;

public class UpdateSubscriptionCommandValidator : AbstractValidator<UpdateSubscriptionCommand>
{
    public UpdateSubscriptionCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Subscription ID is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required");

        RuleFor(x => x.Frequency)
            .IsInEnum()
            .WithMessage("Invalid subscription frequency");
    }
}

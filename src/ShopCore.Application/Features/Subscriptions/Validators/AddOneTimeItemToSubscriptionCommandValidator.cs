using ShopCore.Application.Subscriptions.Commands.AddOneTimeItemToSubscription;

namespace ShopCore.Application.Subscriptions.Validators;

public class AddOneTimeItemToSubscriptionCommandValidator : AbstractValidator<AddOneTimeItemToSubscriptionCommand>
{
    public AddOneTimeItemToSubscriptionCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .GreaterThan(0)
            .WithMessage("Subscription ID is required");

        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.DeliveryDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Delivery date must be today or in the future");

        RuleFor(x => x.Payment)
            .IsInEnum()
            .WithMessage("Invalid payment option");
    }
}

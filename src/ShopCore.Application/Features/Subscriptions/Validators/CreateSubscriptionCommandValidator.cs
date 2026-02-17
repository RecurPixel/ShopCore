using ShopCore.Application.Subscriptions.Commands.CreateSubscription;

namespace ShopCore.Application.Subscriptions.Validators;

public class CreateSubscriptionCommandValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionCommandValidator()
    {
        // VendorId is derived from products in the handler

        RuleFor(x => x.DeliveryAddressId)
            .GreaterThan(0)
            .WithMessage("Delivery address is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required");

        RuleFor(x => x.Frequency)
            .IsInEnum()
            .WithMessage("Invalid subscription frequency");

        RuleFor(x => x.StartDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Start date must be today or in the future");

        RuleFor(x => x.BillingCycleDays)
            .GreaterThan(0)
            .WithMessage("Billing cycle days must be greater than 0");

        RuleFor(x => x.DepositAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DepositAmount.HasValue)
            .WithMessage("Deposit amount cannot be negative");
    }
}

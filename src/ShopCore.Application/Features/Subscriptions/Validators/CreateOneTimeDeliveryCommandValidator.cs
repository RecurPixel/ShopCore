using ShopCore.Application.Subscriptions.Commands.CreateOneTimeDelivery;

namespace ShopCore.Application.Subscriptions.Validators;

public class CreateOneTimeDeliveryCommandValidator : AbstractValidator<CreateOneTimeDeliveryCommand>
{
    public CreateOneTimeDeliveryCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .GreaterThan(0)
            .WithMessage("Vendor ID is required");

        RuleFor(x => x.DeliveryAddressId)
            .GreaterThan(0)
            .WithMessage("Delivery address is required");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required");

        RuleFor(x => x.DeliveryDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Delivery date must be today or in the future");

        RuleFor(x => x.Payment)
            .IsInEnum()
            .WithMessage("Invalid payment option");
    }
}

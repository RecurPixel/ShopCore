using ShopCore.Application.Deliveries.Commands.SkipDelivery;

namespace ShopCore.Application.Deliveries.Validators;

public class SkipDeliveryCommandValidator : AbstractValidator<SkipDeliveryCommand>
{
    public SkipDeliveryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Delivery ID is required");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Reason cannot exceed 500 characters");
    }
}

using ShopCore.Application.Deliveries.Commands.MarkDeliveryFailed;

namespace ShopCore.Application.Deliveries.Validators;

public class MarkDeliveryFailedCommandValidator : AbstractValidator<MarkDeliveryFailedCommand>
{
    public MarkDeliveryFailedCommandValidator()
    {
        RuleFor(x => x.DeliveryId)
            .GreaterThan(0)
            .WithMessage("Delivery ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}

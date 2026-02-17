using ShopCore.Application.Deliveries.Commands.CompleteDelivery;

namespace ShopCore.Application.Deliveries.Validators;

public class CompleteDeliveryCommandValidator : AbstractValidator<CompleteDeliveryCommand>
{
    public CompleteDeliveryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Delivery ID is required");

        RuleFor(x => x.CollectedPaymentReference)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.CollectedPaymentReference))
            .WithMessage("Collected payment reference cannot exceed 100 characters");

        RuleFor(x => x.DeliveryNotes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.DeliveryNotes))
            .WithMessage("Delivery notes cannot exceed 500 characters");
    }
}

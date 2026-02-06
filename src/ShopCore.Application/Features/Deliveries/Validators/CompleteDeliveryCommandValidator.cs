using ShopCore.Application.Deliveries.Commands.CompleteDelivery;

namespace ShopCore.Application.Deliveries.Validators;

public class CompleteDeliveryCommandValidator : AbstractValidator<CompleteDeliveryCommand>
{
    public CompleteDeliveryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Delivery ID is required");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum()
            .When(x => x.PaymentMethod.HasValue)
            .WithMessage("Invalid payment method");

        RuleFor(x => x.PaymentTransactionId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.PaymentTransactionId))
            .WithMessage("Payment transaction ID cannot exceed 100 characters");

        RuleFor(x => x.DeliveryNotes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.DeliveryNotes))
            .WithMessage("Delivery notes cannot exceed 500 characters");
    }
}

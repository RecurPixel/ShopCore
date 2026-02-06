using ShopCore.Application.Orders.Commands.UpdateOrderItemStatus;

namespace ShopCore.Application.Orders.Validators;

public class UpdateOrderItemStatusCommandValidator : AbstractValidator<UpdateOrderItemStatusCommand>
{
    public UpdateOrderItemStatusCommandValidator()
    {
        RuleFor(x => x.OrderItemId)
            .GreaterThan(0)
            .WithMessage("Order item ID is required");

        RuleFor(x => x.NewStatus)
            .IsInEnum()
            .WithMessage("Invalid order item status");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters");

        RuleFor(x => x.TrackingNumber)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.TrackingNumber))
            .WithMessage("Tracking number cannot exceed 100 characters");
    }
}

using ShopCore.Application.Orders.Commands.CancelOrderItem;

namespace ShopCore.Application.Orders.Validators;

public class CancelOrderItemCommandValidator : AbstractValidator<CancelOrderItemCommand>
{
    public CancelOrderItemCommandValidator()
    {
        RuleFor(x => x.OrderItemId)
            .GreaterThan(0)
            .WithMessage("Order item ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}

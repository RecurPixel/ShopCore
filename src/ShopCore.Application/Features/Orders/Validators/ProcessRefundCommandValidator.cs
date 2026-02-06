using ShopCore.Application.Orders.Commands.ProcessRefund;

namespace ShopCore.Application.Orders.Validators;

public class ProcessRefundCommandValidator : AbstractValidator<ProcessRefundCommand>
{
    public ProcessRefundCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order ID is required");

        RuleFor(x => x.OrderItemIds)
            .NotEmpty()
            .WithMessage("At least one order item is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}

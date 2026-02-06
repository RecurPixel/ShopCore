using ShopCore.Application.Vendors.Commands.UpdateVendorOrderStatus;

namespace ShopCore.Application.Vendors.Validators;

public class UpdateVendorOrderStatusCommandValidator : AbstractValidator<UpdateVendorOrderStatusCommand>
{
    public UpdateVendorOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order ID is required");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid order status");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters");
    }
}

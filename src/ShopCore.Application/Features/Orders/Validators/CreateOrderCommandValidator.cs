using ShopCore.Application.Orders.Commands.CreateOrder;

namespace ShopCore.Application.Orders.Validators;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.AddressId).GreaterThan(0).WithMessage("Shipping address is required");

        RuleFor(x => x.PaymentMethod).IsInEnum().WithMessage("Invalid payment method");
    }
}

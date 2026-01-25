namespace ShopCore.Application.Features.Orders.Validators;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.AddressId)
            .GreaterThan(0).WithMessage("Shipping address is required");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method");
    }
}

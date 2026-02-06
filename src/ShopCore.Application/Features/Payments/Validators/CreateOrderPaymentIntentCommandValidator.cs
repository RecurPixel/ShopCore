using ShopCore.Application.Payments.Commands.CreateOrderPaymentIntent;

namespace ShopCore.Application.Payments.Validators;

public class CreateOrderPaymentIntentCommandValidator : AbstractValidator<CreateOrderPaymentIntentCommand>
{
    public CreateOrderPaymentIntentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order ID is required");
    }
}

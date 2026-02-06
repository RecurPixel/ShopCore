using ShopCore.Application.Payments.Commands.RecordCODPayment;

namespace ShopCore.Application.Payments.Validators;

public class RecordCODPaymentCommandValidator : AbstractValidator<RecordCODPaymentCommand>
{
    public RecordCODPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order ID is required");
    }
}

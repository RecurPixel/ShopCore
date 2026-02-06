using ShopCore.Application.Payments.Commands.CreateInvoicePaymentIntent;

namespace ShopCore.Application.Payments.Validators;

public class CreateInvoicePaymentIntentCommandValidator : AbstractValidator<CreateInvoicePaymentIntentCommand>
{
    public CreateInvoicePaymentIntentCommandValidator()
    {
        RuleFor(x => x.InvoiceId)
            .GreaterThan(0)
            .WithMessage("Invoice ID is required");
    }
}

using ShopCore.Application.Invoices.Commands.PayInvoice;

namespace ShopCore.Application.Invoices.Validators;

public class PayInvoiceCommandValidator : AbstractValidator<PayInvoiceCommand>
{
    public PayInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceId)
            .GreaterThan(0)
            .WithMessage("Invoice ID is required");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum()
            .WithMessage("Invalid payment method");

        RuleFor(x => x.PaymentTransactionId)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.PaymentTransactionId))
            .WithMessage("Payment transaction ID cannot exceed 100 characters");
    }
}

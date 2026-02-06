using ShopCore.Application.Invoices.Commands.GenerateSubscriptionInvoice;

namespace ShopCore.Application.Invoices.Validators;

public class GenerateSubscriptionInvoiceCommandValidator : AbstractValidator<GenerateSubscriptionInvoiceCommand>
{
    public GenerateSubscriptionInvoiceCommandValidator()
    {
        RuleFor(x => x.SubscriptionId)
            .GreaterThan(0)
            .WithMessage("Subscription ID is required");
    }
}

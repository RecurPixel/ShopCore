using ShopCore.Application.Payments.Commands.HandlePaymentWebhook;

namespace ShopCore.Application.Payments.Validators;

public class HandlePaymentWebhookCommandValidator : AbstractValidator<HandlePaymentWebhookCommand>
{
    public HandlePaymentWebhookCommandValidator()
    {
        RuleFor(x => x.Gateway)
            .IsInEnum()
            .WithMessage("Invalid payment gateway");

        RuleFor(x => x.Payload)
            .NotEmpty()
            .WithMessage("Webhook payload is required");

        // Signature is optional as some gateways verify differently
    }
}

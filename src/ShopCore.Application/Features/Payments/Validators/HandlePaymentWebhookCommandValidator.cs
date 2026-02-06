using ShopCore.Application.Payments.Commands.HandlePaymentWebhook;

namespace ShopCore.Application.Payments.Validators;

public class HandlePaymentWebhookCommandValidator : AbstractValidator<HandlePaymentWebhookCommand>
{
    public HandlePaymentWebhookCommandValidator()
    {
        RuleFor(x => x.Payload)
            .NotEmpty()
            .WithMessage("Webhook payload is required");

        RuleFor(x => x.Signature)
            .NotEmpty()
            .WithMessage("Webhook signature is required");
    }
}

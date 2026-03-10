using ShopCore.Application.Payments.Commands.ConfirmPayment;

namespace ShopCore.Application.Payments.Validators;

public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator()
    {
        RuleFor(x => x.Gateway)
            .IsInEnum()
            .WithMessage("Invalid payment gateway");

        RuleFor(x => x.GatewayOrderId)
            .NotEmpty()
            .WithMessage("Gateway order ID is required");

        RuleFor(x => x.GatewayPaymentId)
            .NotEmpty()
            .WithMessage("Gateway payment ID is required");

        // Signature is required for Razorpay
        RuleFor(x => x.Signature)
            .NotEmpty()
            .When(x => x.Gateway == PaymentGateway.Razorpay)
            .WithMessage("Signature is required for Razorpay payments");
    }
}

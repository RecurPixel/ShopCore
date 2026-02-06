using ShopCore.Application.Payments.Commands.ConfirmPayment;

namespace ShopCore.Application.Payments.Validators;

public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator()
    {
        RuleFor(x => x.RazorpayOrderId)
            .NotEmpty()
            .WithMessage("Razorpay order ID is required");

        RuleFor(x => x.RazorpayPaymentId)
            .NotEmpty()
            .WithMessage("Razorpay payment ID is required");

        RuleFor(x => x.RazorpaySignature)
            .NotEmpty()
            .WithMessage("Razorpay signature is required");
    }
}

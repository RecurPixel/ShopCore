using ShopCore.Application.CustomerInvitations.Commands.CreateCustomerInvitation;

namespace ShopCore.Application.CustomerInvitations.Validators;

public class CreateCustomerInvitationCommandValidator : AbstractValidator<CreateCustomerInvitationCommand>
{
    public CreateCustomerInvitationCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .WithMessage("Customer name is required")
            .MaximumLength(200);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^[6-9]\d{9}$")
            .WithMessage("Phone number must be a valid Indian mobile number");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Invalid email address");

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty()
            .WithMessage("Delivery address is required")
            .MaximumLength(500);

        RuleFor(x => x.Pincode)
            .NotEmpty()
            .WithMessage("Pincode is required")
            .Matches(@"^\d{6}$")
            .WithMessage("Pincode must be 6 digits");

        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required");

        RuleFor(x => x.PreferredDeliveryTime)
            .NotEmpty()
            .WithMessage("Preferred delivery time is required")
            .Matches(@"^([01]\d|2[0-3]):([0-5]\d)$")
            .WithMessage("Time must be in HH:mm format");

        RuleFor(x => x.DepositAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DepositAmount.HasValue)
            .WithMessage("Deposit amount cannot be negative");

        RuleFor(x => x)
            .Must(x => x.SendSms || x.SendWhatsApp || x.SendEmail)
            .WithMessage("At least one notification method must be selected");
    }
}

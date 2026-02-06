using ShopCore.Application.Vendors.Commands.UpdateMyVendor;

namespace ShopCore.Application.Vendors.Validators;

public class UpdateMyVendorCommandValidator : AbstractValidator<UpdateMyVendorCommand>
{
    public UpdateMyVendorCommandValidator()
    {
        RuleFor(x => x.BusinessName)
            .NotEmpty()
            .WithMessage("Business name is required")
            .MaximumLength(200)
            .WithMessage("Business name cannot exceed 200 characters");

        RuleFor(x => x.BusinessAddress)
            .NotEmpty()
            .WithMessage("Business address is required")
            .MaximumLength(500)
            .WithMessage("Business address cannot exceed 500 characters");

        RuleFor(x => x.BankName)
            .NotEmpty()
            .WithMessage("Bank name is required")
            .MaximumLength(100)
            .WithMessage("Bank name cannot exceed 100 characters");

        RuleFor(x => x.BankAccountNumber)
            .NotEmpty()
            .WithMessage("Bank account number is required")
            .MaximumLength(20)
            .WithMessage("Bank account number cannot exceed 20 characters");

        RuleFor(x => x.BankIfscCode)
            .NotEmpty()
            .WithMessage("Bank IFSC code is required")
            .MaximumLength(11)
            .WithMessage("Bank IFSC code cannot exceed 11 characters");

        RuleFor(x => x.BankAccountHolderName)
            .NotEmpty()
            .WithMessage("Bank account holder name is required")
            .MaximumLength(100)
            .WithMessage("Bank account holder name cannot exceed 100 characters");

        RuleFor(x => x.DefaultDepositAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DefaultDepositAmount.HasValue)
            .WithMessage("Default deposit amount cannot be negative");

        RuleFor(x => x.DefaultBillingCycleDays)
            .GreaterThan(0)
            .When(x => x.DefaultBillingCycleDays.HasValue)
            .WithMessage("Default billing cycle days must be greater than 0");
    }
}

using ShopCore.Application.Vendors.Commands.UploadVendorLogo;

namespace ShopCore.Application.Vendors.Validators;

public class UploadVendorLogoCommandValidator : AbstractValidator<UploadVendorLogoCommand>
{
    public UploadVendorLogoCommandValidator()
    {
        RuleFor(x => x.AvatarFile)
            .NotNull()
            .WithMessage("Logo file is required");
    }
}

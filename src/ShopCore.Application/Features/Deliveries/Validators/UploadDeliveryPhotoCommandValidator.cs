using ShopCore.Application.Deliveries.Commands.UploadDeliveryPhoto;

namespace ShopCore.Application.Deliveries.Validators;

public class UploadDeliveryPhotoCommandValidator : AbstractValidator<UploadDeliveryPhotoCommand>
{
    public UploadDeliveryPhotoCommandValidator()
    {
        RuleFor(x => x.DeliveryId)
            .GreaterThan(0)
            .WithMessage("Delivery ID is required");

        RuleFor(x => x.Photo)
            .NotNull()
            .WithMessage("Photo is required");
    }
}

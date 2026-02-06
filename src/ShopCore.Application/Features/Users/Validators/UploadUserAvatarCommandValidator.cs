using ShopCore.Application.Users.Commands.UploadUserAvatar;

namespace ShopCore.Application.Users.Validators;

public class UploadUserAvatarCommandValidator : AbstractValidator<UploadUserAvatarCommand>
{
    public UploadUserAvatarCommandValidator()
    {
        RuleFor(x => x.AvatarFile)
            .NotNull()
            .WithMessage("Avatar file is required");
    }
}

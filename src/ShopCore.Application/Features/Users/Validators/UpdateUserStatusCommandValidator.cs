using ShopCore.Application.Users.Commands.UpdateUserStatus;

namespace ShopCore.Application.Users.Validators;

public class UpdateUserStatusCommandValidator : AbstractValidator<UpdateUserStatusCommand>
{
    public UpdateUserStatusCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0)
            .WithMessage("User ID is required");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid user status");
    }
}

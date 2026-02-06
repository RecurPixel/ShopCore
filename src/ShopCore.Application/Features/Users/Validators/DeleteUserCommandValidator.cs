using ShopCore.Application.Users.Commands.DeleteUser;

namespace ShopCore.Application.Users.Validators;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("User ID is required");
    }
}

namespace ShopCore.Application.Users.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    public Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}

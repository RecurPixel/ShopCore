namespace ShopCore.Application.Auth.Commands.ForgotPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    public Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}

namespace ShopCore.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand>
{
    public Task Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}

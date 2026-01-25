using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    public Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.FromResult(new LoginResponse());
    }
}

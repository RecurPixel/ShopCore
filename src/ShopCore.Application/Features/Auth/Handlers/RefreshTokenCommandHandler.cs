using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    public Task<RefreshTokenResponse> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new RefreshTokenResponse());
    }
}

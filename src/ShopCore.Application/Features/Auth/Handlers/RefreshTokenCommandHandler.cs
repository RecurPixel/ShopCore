using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtService;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IJwtTokenService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<RefreshTokenResponse> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken
    )
    {

        // Find user by refresh token
        var user = await _context.Users
            .Include(u => u.VendorProfile)
            .FirstOrDefaultAsync(
                u => u.RefreshToken == request.RefreshToken,
                cancellationToken);

        if (user == null)
            throw new UnauthorizedException("Invalid refresh token");

        // Check if refresh token is expired
        if (user.RefreshTokenExpiry == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            throw new UnauthorizedException("Refresh token has expired");

        // Check if user is active
        if (!user.IsActive)
            throw new UnauthorizedException("Account is deactivated");

        // Generate new tokens
        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Update refresh token in database
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 3600 // 1 hour
        };

    }
}

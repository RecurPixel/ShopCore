using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTime _dateTime;

    public RefreshTokenCommandHandler(
        IApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IDateTime dateTime)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _dateTime = dateTime;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        // 1. Find user by refresh token
        var user = await _context.Users
            .Include(u => u.VendorProfile)
            .FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken, ct);

        if (user == null || user.RefreshTokenExpiry < _dateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token");

        // 2. Generate new tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // 3. Update user
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = _dateTime.UtcNow.AddDays(7);
        await _context.SaveChangesAsync(ct);

        return new RefreshTokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = 3600
        };
    }

}
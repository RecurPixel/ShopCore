using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{

    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IDateTime _dateTime;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IDateTime dateTime)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _dateTime = dateTime;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        // 1. Find user by email (include VendorProfile for VendorId)
        var user = await _context.Users
            .Include(u => u.VendorProfile)
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), ct);

        if (user == null)
            throw new UnauthorizedException("Invalid email or password");

        // 2. Verify password
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password");

        // 3. Check if account is active
        if (!user.IsActive)
            throw new UnauthorizedException("Account is inactive");

        // 4. Generate tokens
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // 5. Update user
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = _dateTime.UtcNow.AddDays(7);
        user.LastLoginAt = _dateTime.UtcNow;
        await _context.SaveChangesAsync(ct);

        return new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = 3600,
            User = MapToUserDto(user)
        };
    }

    private UserDto MapToUserDto(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Role = user.Role.ToString(),
        IsEmailVerified = user.IsEmailVerified,
        AvatarUrl = user.AvatarUrl
    };

}

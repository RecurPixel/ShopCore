using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly INotificationService _notificationService;
    private readonly IDateTime _dateTime;

    public RegisterUserCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        INotificationService notificationService,
        IDateTime dateTime)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _notificationService = notificationService;
        _dateTime = dateTime;
    }

    public async Task<RegisterResponse> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        // 1. Check if email already exists
        var emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email, ct);
        if (emailExists)
            throw new ValidationException("Email already registered");

        // 2. Check if phone already exists
        var phoneExists = await _context.Users.AnyAsync(u => u.PhoneNumber == request.PhoneNumber, ct);
        if (phoneExists)
            throw new ValidationException("Phone number already registered");

        // 3. Hash password
        var passwordHash = _passwordHasher.Hash(request.Password);

        // 4. Generate email verification token
        var verificationToken = Guid.NewGuid().ToString();

        // 5. Create user (Role defaults to Customer, upgraded to Vendor when vendor profile is created/approved)
        var user = new User
        {
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = UserRole.Customer,
            IsEmailVerified = false,
            EmailVerificationToken = verificationToken,
            EmailVerificationTokenExpiry = _dateTime.UtcNow.AddHours(24),
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);

        // 6. Send welcome notification (fire and forget — failures are logged internally)
        var verifyUrl = "https://shopcore.com/verify-email";
        await _notificationService.SendWelcomeAsync(user);
        await _notificationService.SendEmailVerificationAsync(user, verificationToken, verifyUrl);

        return new RegisterResponse
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role.ToString()
        };
    }
}

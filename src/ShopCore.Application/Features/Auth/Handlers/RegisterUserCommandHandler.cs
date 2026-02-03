using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IDateTime _dateTime;

    public RegisterUserCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
        IDateTime dateTime)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
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

        // 5. Create user
        var user = new User
        {
            Email = request.Email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            IsEmailVerified = false,
            EmailVerificationToken = verificationToken,
            EmailVerificationTokenExpiry = _dateTime.UtcNow.AddHours(24),
            IsActive = true
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(ct);

        // 6. Send verification email (fire and forget)
        await _context.SaveChangesAsync(ct);

        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(
            user.Email,
            user.FullName
        );

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

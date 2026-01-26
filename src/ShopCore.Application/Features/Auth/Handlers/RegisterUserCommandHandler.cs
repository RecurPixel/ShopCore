using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTime _dateTime;

    private readonly IEmailService _emailService;

    public RegisterUserCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IDateTime dateTime,
        IEmailService emailService
    )
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _dateTime = dateTime;
        _emailService = emailService;
    }

    public async Task<RegisterResponse> Handle(
        RegisterUserCommand request,
        CancellationToken cancellationToken
    )
    {
        // Check if user already exists
        var exists = await _context.Users.AnyAsync(
            u => u.Email == request.Email,
            cancellationToken
        );

        if (exists)
            throw new ValidationException("Email already registered.");

        // Hash password
        var passwordHash = _passwordHasher.Hash(request.Password);

        // Create user entity
        var user = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,  // ← Store hashed password
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(
            user.Email,
            user.FullName
        );

        // Send email verification
        var verificationUrl = "https://shopcore.com/verify-email";
        await _emailService.SendEmailVerificationAsync(
            user.Email,
            user.EmailVerificationToken!,
            verificationUrl
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

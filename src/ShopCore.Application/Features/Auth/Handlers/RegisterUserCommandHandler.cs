using ShopCore.Application.Common.Interfaces;
using ShopCore.Domain.Entities;

namespace ShopCore.Application.Auth.Commands.RegisterUser;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTime _dateTime;

    public RegisterUserCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IDateTime dateTime
    )
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _dateTime = dateTime;
    }

    public async Task Handle(RegisterUserCommand request, CancellationToken cancellationToken)
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
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = passwordHash,
            CreatedAt = _dateTime.UtcNow,
            IsActive = true,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

using ShopCore.Application.Auth.Commands.ResendVerification;

namespace ShopCore.Application.Auth.Commands.ResendVerification;

public class ResendVerificationCommandHandler : IRequestHandler<ResendVerificationCommand>
{
    private readonly IEmailService _emailService;
    private readonly IApplicationDbContext _context;

    public ResendVerificationCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task Handle(ResendVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || user.IsEmailVerified)
            return;

        // Generate new verification token
        user.EmailVerificationToken = Guid.NewGuid().ToString();
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(24);

        await _context.SaveChangesAsync(cancellationToken);

        // Send verification email
        var verifyUrl = "https://shopcore.com/verify-email";
        await _emailService.SendEmailVerificationAsync(
            user.Email,
            user.EmailVerificationToken,
            verifyUrl
        );
    }
}

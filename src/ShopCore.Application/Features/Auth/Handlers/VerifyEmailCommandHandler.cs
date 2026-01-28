namespace ShopCore.Application.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand>
{
    private readonly IApplicationDbContext _context;
    public VerifyEmailCommandHandler(
        IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        // Find user by reset token
        var user = await _context.Users
            .FirstOrDefaultAsync(
                u => u.EmailVerificationToken == request.VerificationToken &&
                     u.EmailVerificationTokenExpiry > DateTime.UtcNow,
                cancellationToken);

        if (user == null)
            throw new ValidationException("Invalid or expired verification token");


        // Verify email

        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

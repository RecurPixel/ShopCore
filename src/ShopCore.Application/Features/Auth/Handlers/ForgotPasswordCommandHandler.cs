namespace ShopCore.Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IEmailService _emailService;

    private readonly IApplicationDbContext _context;

    public ForgotPasswordCommandHandler(
        IApplicationDbContext context,
        IEmailService emailService
    )
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null)
            return;

        // Generate reset token
        user.PasswordResetToken = Guid.NewGuid().ToString();
        user.PasswordResetExpiry = DateTime.UtcNow.AddHours(1);

        await _context.SaveChangesAsync(cancellationToken);

        // Send password reset email
        var resetUrl = "https://shopcore.com/reset-password";
        await _emailService.SendPasswordResetEmailAsync(
            user.Email,
            user.PasswordResetToken,
            resetUrl
        );
    }
}

namespace ShopCore.Application.Auth.Commands.ForgotPassword;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(
        ResetPasswordCommand request,
        CancellationToken cancellationToken)
    {
        // Find user by reset token
        var user = await _context.Users
            .FirstOrDefaultAsync(
                u => u.PasswordResetToken == request.ResetToken &&
                     u.PasswordResetExpiry > DateTime.UtcNow,
                cancellationToken);

        if (user == null)
            throw new ValidationException("Invalid or expired reset token");

        // Hash new password
        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

        // Clear reset token
        user.PasswordResetToken = null;
        user.PasswordResetExpiry = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
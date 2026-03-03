using ShopCore.Application.Auth.Commands.ResendVerification;

namespace ShopCore.Application.Auth.Commands.ResendVerification;

public class ResendVerificationCommandHandler : IRequestHandler<ResendVerificationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public ResendVerificationCommandHandler(
        IApplicationDbContext context,
        INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
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

        var verifyUrl = "https://shopcore.com/verify-email";
        await _notificationService.SendEmailVerificationAsync(user, user.EmailVerificationToken!, verifyUrl);
    }
}

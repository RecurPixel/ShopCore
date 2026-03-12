namespace ShopCore.Application.Auth.Commands.ResendVerification;

public class ResendVerificationCommandHandler : IRequestHandler<ResendVerificationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IConfiguration _configuration;

    public ResendVerificationCommandHandler(
        IApplicationDbContext context,
        INotificationService notificationService,
        IConfiguration configuration)
    {
        _context = context;
        _notificationService = notificationService;
        _configuration = configuration;
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

        var baseUrl = (_configuration["App:BaseUrl"] ?? "https://localhost:7000").TrimEnd('/');
        var verifyUrl = $"{baseUrl}/verify-email.html";
        await _notificationService.SendEmailVerificationAsync(user, user.EmailVerificationToken!, verifyUrl);
    }
}

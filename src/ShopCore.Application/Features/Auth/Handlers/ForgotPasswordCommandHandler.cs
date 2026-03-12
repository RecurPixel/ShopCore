namespace ShopCore.Application.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IConfiguration _configuration;

    public ForgotPasswordCommandHandler(
        IApplicationDbContext context,
        INotificationService notificationService,
        IConfiguration configuration)
    {
        _context = context;
        _notificationService = notificationService;
        _configuration = configuration;
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

        var baseUrl = (_configuration["App:BaseUrl"] ?? "https://localhost:7001").TrimEnd('/');
        var resetUrl = $"{baseUrl}/reset-password.html";
        await _notificationService.SendPasswordResetAsync(user, user.PasswordResetToken, resetUrl);
    }
}

namespace ShopCore.Application.Payouts.Commands.ProcessPayout;

public class ProcessPayoutCommandHandler : IRequestHandler<ProcessPayoutCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;

    public ProcessPayoutCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task Handle(ProcessPayoutCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can process payouts");

        var payout = await _context.VendorPayouts.FindAsync(request.PayoutId);
        if (payout == null)
            throw new NotFoundException("Payout", request.PayoutId);

        if (payout.Status != PayoutStatus.Pending)
            throw new ValidationException("Only pending payouts can be processed");

        payout.Status = PayoutStatus.Paid;
        payout.PaidAt = _dateTime.UtcNow;
        payout.ProcessedBy = _currentUser.UserId;

        await _context.SaveChangesAsync(ct);
    }
}

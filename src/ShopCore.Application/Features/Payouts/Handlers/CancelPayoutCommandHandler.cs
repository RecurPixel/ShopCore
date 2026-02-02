namespace ShopCore.Application.Payouts.Commands.CancelPayout;

public class CancelPayoutCommandHandler : IRequestHandler<CancelPayoutCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CancelPayoutCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(CancelPayoutCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can cancel payouts");

        var payout = await _context.VendorPayouts.FindAsync(request.PayoutId);
        if (payout == null)
            throw new NotFoundException("Payout", request.PayoutId);

        if (payout.Status == PayoutStatus.Paid)
            throw new ValidationException("Cannot cancel paid payouts");

        payout.Status = PayoutStatus.Cancelled;
        await _context.SaveChangesAsync(ct);
    }
}

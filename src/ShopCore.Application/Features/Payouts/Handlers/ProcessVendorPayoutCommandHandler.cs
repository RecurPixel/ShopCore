using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.ProcessVendorPayout;

public class ProcessVendorPayoutCommandHandler
    : IRequestHandler<ProcessVendorPayoutCommand, VendorPayoutDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;

    public ProcessVendorPayoutCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<VendorPayoutDto> Handle(ProcessVendorPayoutCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can process payouts");

        var payout = await _context.VendorPayouts
            .Include(p => p.Vendor)
            .FirstOrDefaultAsync(p => p.Id == request.PayoutId, ct);

        if (payout == null)
            throw new NotFoundException("Payout", request.PayoutId);

        payout.Status = PayoutStatus.Paid;
        payout.PaidAt = _dateTime.UtcNow;
        payout.PayoutTransactionId = request.TransactionId;
        payout.TransactionReference = request.Notes;
        payout.ProcessedBy = _currentUser.UserId;

        await _context.SaveChangesAsync(ct);

        return new VendorPayoutDto
        {
            Id = payout.Id,
            VendorId = payout.VendorId,
            VendorName = payout.Vendor.BusinessName,
            PeriodFrom = payout.PeriodStart,
            PeriodTo = payout.PeriodEnd,
            GrossAmount = payout.TotalSales,
            PlatformFee = payout.CommissionAmount,
            NetAmount = payout.NetPayout,
            Status = payout.Status.ToString(),
            PayoutMethod = payout.PayoutMethod?.ToString(),
            PayoutTransactionId = payout.PayoutTransactionId,
            ProcessedAt = payout.PaidAt
        };
    }
}

using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CreatePayout;

public class CreatePayoutCommandHandler : IRequestHandler<CreatePayoutCommand, PayoutDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;

    public CreatePayoutCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<PayoutDto> Handle(CreatePayoutCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can create payouts");

        var vendor = await _context.VendorProfiles.FindAsync(request.VendorId);
        if (vendor == null)
            throw new NotFoundException("Vendor", request.VendorId);

        var today = _dateTime.UtcNow.Date;
        var payoutNumber = await GeneratePayoutNumberAsync(today, ct);

        var payout = new VendorPayout
        {
            VendorId = request.VendorId,
            PayoutNumber = payoutNumber,
            TotalSales = request.Amount,
            CommissionAmount = 0,
            DeductionsAmount = 0,
            NetPayout = request.Amount,
            Status = PayoutStatus.Pending,
            PeriodStart = today.AddDays(-30),
            PeriodEnd = today,
            ProcessedBy = _currentUser.UserId
        };

        _context.VendorPayouts.Add(payout);
        await _context.SaveChangesAsync(ct);

        return new PayoutDto
        {
            Id = payout.Id,
            VendorId = payout.VendorId,
            VendorName = vendor.BusinessName,
            Amount = payout.NetPayout,
            Status = payout.Status.ToString(),
            TransactionReference = payout.TransactionReference,
            CreatedAt = payout.CreatedAt,
            ProcessedAt = payout.PaidAt
        };
    }

    private async Task<string> GeneratePayoutNumberAsync(DateTime date, CancellationToken ct)
    {
        var prefix = $"PAYOUT-{date:yyyyMMdd}";
        var count = await _context.VendorPayouts
            .Where(p => p.PayoutNumber.StartsWith(prefix))
            .CountAsync(ct);
        return $"{prefix}-{(count + 1):D4}";
    }
}

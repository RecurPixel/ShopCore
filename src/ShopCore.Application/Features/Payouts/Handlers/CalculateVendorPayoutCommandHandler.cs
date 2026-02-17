using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CalculateVendorPayout;

public class CalculateVendorPayoutCommandHandler
    : IRequestHandler<CalculateVendorPayoutCommand, VendorPayoutDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CalculateVendorPayoutCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<VendorPayoutDto> Handle(CalculateVendorPayoutCommand request, CancellationToken ct)
    {
        // Determine VendorId: admins can specify, vendors use their own
        int vendorId;
        if (request.VendorId.HasValue && request.VendorId.Value > 0)
        {
            // Only admins can calculate payouts for other vendors
            if (_currentUser.Role != UserRole.Admin && _currentUser.VendorId != request.VendorId.Value)
                throw new ForbiddenException("You can only calculate your own payouts");
            vendorId = request.VendorId.Value;
        }
        else
        {
            // Use authenticated vendor's ID
            if (!_currentUser.VendorId.HasValue)
                throw new ForbiddenException("Vendor account required");
            vendorId = _currentUser.VendorId.Value;
        }

        var vendor = await _context.VendorProfiles.FindAsync(vendorId);
        if (vendor == null)
            throw new NotFoundException("Vendor", vendorId);

        // Calculate from delivered orders in period
        var deliveredOrders = await _context.OrderItems
            .Where(oi => oi.VendorId == vendorId &&
                       oi.Status == OrderItemStatus.Delivered &&
                       oi.Order.DeliveredAt >= request.FromDate &&
                       oi.Order.DeliveredAt <= request.ToDate)
            .ToListAsync(ct);

        var totalSales = deliveredOrders.Sum(oi => oi.Subtotal);
        var commission = deliveredOrders.Sum(oi => oi.CommissionAmount);
        var netPayout = totalSales - commission;

        return new VendorPayoutDto
        {
            VendorId = vendorId,
            VendorName = vendor.BusinessName,
            PeriodFrom = request.FromDate,
            PeriodTo = request.ToDate,
            GrossAmount = totalSales,
            PlatformFee = commission,
            NetAmount = netPayout,
            Status = "Calculated"
        };
    }
}

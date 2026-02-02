using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CalculateVendorPayout;

public class CalculateVendorPayoutCommandHandler
    : IRequestHandler<CalculateVendorPayoutCommand, VendorPayoutDto>
{
    private readonly IApplicationDbContext _context;

    public CalculateVendorPayoutCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<VendorPayoutDto> Handle(CalculateVendorPayoutCommand request, CancellationToken ct)
    {
        var vendor = await _context.VendorProfiles.FindAsync(request.VendorId);
        if (vendor == null)
            throw new NotFoundException("Vendor", request.VendorId);

        // Calculate from delivered orders in period
        var deliveredOrders = await _context.OrderItems
            .Where(oi => oi.VendorId == request.VendorId &&
                       oi.Status == OrderStatus.Delivered &&
                       oi.Order.DeliveredAt >= request.FromDate &&
                       oi.Order.DeliveredAt <= request.ToDate)
            .ToListAsync(ct);

        var totalSales = deliveredOrders.Sum(oi => oi.Subtotal);
        var commission = deliveredOrders.Sum(oi => oi.CommissionAmount);
        var netPayout = totalSales - commission;

        return new VendorPayoutDto
        {
            VendorId = request.VendorId,
            VendorName = vendor.BusinessName,
            PeriodStart = request.FromDate,
            PeriodEnd = request.ToDate,
            TotalSales = totalSales,
            CommissionAmount = commission,
            NetPayout = netPayout,
            OrderCount = deliveredOrders.Count
        };
    }
}

using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CalculatePendingPayouts;

public class CalculatePendingPayoutsCommandHandler : IRequestHandler<CalculatePendingPayoutsCommand, PendingPayoutSummaryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;

    public CalculatePendingPayoutsCommandHandler(IApplicationDbContext context, IDateTime dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<PendingPayoutSummaryDto> Handle(CalculatePendingPayoutsCommand request, CancellationToken ct)
    {
        var thirtyDaysAgo = _dateTime.UtcNow.AddDays(-30);

        var deliveredOrders = await _context.OrderItems
            .Include(oi => oi.Vendor)
            .Where(oi => oi.Status == OrderItemStatus.Delivered &&
                       oi.Order.DeliveredAt >= thirtyDaysAgo)
            .GroupBy(oi => new { oi.VendorId, oi.Vendor.BusinessName })
            .Select(g => new VendorPendingPayoutDto
            {
                VendorId = g.Key.VendorId,
                VendorName = g.Key.BusinessName,
                PendingAmount = g.Sum(oi => oi.Subtotal) - g.Sum(oi => oi.CommissionAmount),
                OrderCount = g.Select(oi => oi.OrderId).Distinct().Count(),
                OldestOrderDate = g.Min(oi => oi.Order.DeliveredAt)
            })
            .ToListAsync(ct);

        var vendorPayouts = deliveredOrders;

        return new PendingPayoutSummaryDto
        {
            VendorPayouts = vendorPayouts,
            TotalPayoutAmount = vendorPayouts.Sum(v => v.PendingAmount),
            VendorCount = vendorPayouts.Count
        };
    }
}

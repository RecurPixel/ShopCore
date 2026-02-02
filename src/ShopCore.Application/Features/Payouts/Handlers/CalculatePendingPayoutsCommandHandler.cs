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
            .Where(oi => oi.Status == OrderStatus.Delivered &&
                       oi.Order.DeliveredAt >= thirtyDaysAgo)
            .GroupBy(oi => oi.VendorId)
            .Select(g => new
            {
                VendorId = g.Key,
                TotalSales = g.Sum(oi => oi.Subtotal),
                Commission = g.Sum(oi => oi.CommissionAmount)
            })
            .ToListAsync(ct);

        var vendorPayouts = deliveredOrders.Select(x => new VendorPayoutSummaryDto
        {
            VendorId = x.VendorId,
            TotalSales = x.TotalSales,
            Commission = x.Commission,
            NetPayout = x.TotalSales - x.Commission
        }).ToList();

        return new PendingPayoutSummaryDto
        {
            VendorPayouts = vendorPayouts,
            TotalPayoutAmount = vendorPayouts.Sum(v => v.NetPayout),
            VendorCount = vendorPayouts.Count
        };
    }
}

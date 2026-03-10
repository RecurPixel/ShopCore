using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerById;

public class GetVendorCustomerByIdQueryHandler : IRequestHandler<GetVendorCustomerByIdQuery, VendorCustomerDetailDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorCustomerByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<VendorCustomerDetailDto?> Handle(
        GetVendorCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var vendorId = _currentUser.VendorId;

        // Check if customer has any relationship with this vendor
        var hasRelationship = await _context.OrderItems
            .AnyAsync(oi => oi.Order.UserId == request.UserId
                && oi.VendorId == vendorId, cancellationToken) ||
            await _context.Subscriptions
            .AnyAsync(s => s.UserId == request.UserId
                && s.VendorId == vendorId, cancellationToken);

        if (!hasRelationship)
            return null;

        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
            return null;

        var vendorOrderItems = _context.OrderItems
            .Where(oi => oi.Order.UserId == request.UserId && oi.VendorId == vendorId);

        var paidOrderItems = vendorOrderItems
            .Where(oi => oi.Order.PaymentStatus == PaymentStatus.Paid);

        var deliveredOrderItems = vendorOrderItems
            .Where(oi => oi.Status == OrderItemStatus.Delivered);

        return new VendorCustomerDetailDto
        {
            UserId = user.Id,
            FullName = user.FirstName + " " + user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            DefaultAddress = user.Addresses
                .Where(a => a.IsDefault)
                .Select(a => $"{a.AddressLine1}, {a.City}")
                .FirstOrDefault(),
            ActiveSubscriptionCount = await _context.Subscriptions
                .CountAsync(s => s.UserId == request.UserId
                    && s.VendorId == vendorId
                    && s.Status == SubscriptionStatus.Active, cancellationToken),
            TotalOrderCount = await vendorOrderItems.CountAsync(cancellationToken),
            TotalDeliveryCount = await deliveredOrderItems.CountAsync(cancellationToken),
            TotalSpent = await paidOrderItems.SumAsync(oi => oi.Quantity * oi.UnitPrice, cancellationToken),
            FirstOrderDate = await _context.Orders
                .Where(o => o.UserId == request.UserId
                    && o.Items.Any(oi => oi.VendorId == vendorId))
                .OrderBy(o => o.CreatedAt)
                .Select(o => (DateTime?)o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken),
            LastOrderDate = await _context.Orders
                .Where(o => o.UserId == request.UserId
                    && o.Items.Any(oi => oi.VendorId == vendorId))
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => (DateTime?)o.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken),
            LastDeliveryDate = await deliveredOrderItems
                .OrderByDescending(oi => oi.DeliveredAt)
                .Select(oi => oi.DeliveredAt)
                .FirstOrDefaultAsync(cancellationToken)
        };
    }
}

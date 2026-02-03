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
        // Check if customer has any relationship with this vendor
        var hasRelationship = await _context.OrderItems
            .AnyAsync(oi => oi.Order.UserId == request.UserId
                && oi.VendorId == _currentUser.VendorId, cancellationToken) ||
            await _context.Subscriptions
            .AnyAsync(s => s.UserId == request.UserId
                && s.VendorId == _currentUser.VendorId, cancellationToken);

        if (!hasRelationship)
            return null;

        var user = await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == request.UserId)
            .Select(u => new VendorCustomerDetailDto
            {
                UserId = u.Id,
                Name = u.FirstName + " " + u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                JoinedDate = u.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
            return null;

        // Get order stats
        user.TotalOrders = await _context.OrderItems
            .CountAsync(oi => oi.Order.UserId == request.UserId
                && oi.VendorId == _currentUser.VendorId, cancellationToken);

        user.TotalSpent = await _context.OrderItems
            .Where(oi => oi.Order.UserId == request.UserId
                && oi.VendorId == _currentUser.VendorId
                && oi.Order.PaymentStatus == PaymentStatus.Paid)
            .SumAsync(oi => oi.Subtotal, cancellationToken);

        // Get subscription stats
        user.ActiveSubscriptions = await _context.Subscriptions
            .CountAsync(s => s.UserId == request.UserId
                && s.VendorId == _currentUser.VendorId
                && s.Status == SubscriptionStatus.Active, cancellationToken);

        user.LastOrderDate = await _context.Orders
            .Where(o => o.UserId == request.UserId
                && o.Items.Any(oi => oi.VendorId == _currentUser.VendorId))
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => (DateTime?)o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        return user;
    }
}

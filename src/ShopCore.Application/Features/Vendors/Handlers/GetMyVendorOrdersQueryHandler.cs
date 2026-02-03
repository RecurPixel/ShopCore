using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetMyVendorOrders;

public class GetMyVendorOrdersQueryHandler
    : IRequestHandler<GetMyVendorOrdersQuery, List<VendorOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMyVendorOrdersQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<VendorOrderDto>> Handle(
        GetMyVendorOrdersQuery request,
        CancellationToken cancellationToken)
    {
        // Get orders that contain items from this vendor
        var orders = await _context.Orders
            .AsNoTracking()
            .Include(o => o.User)
            .Include(o => o.Items
                .Where(oi => oi.VendorId == _currentUser.VendorId))
            .Where(o => o.Items.Any(oi => oi.VendorId == _currentUser.VendorId))
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new VendorOrderDto
            {
                OrderId = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.User.FirstName + " " + o.User.LastName,
                CustomerPhone = o.User.PhoneNumber,
                OrderDate = o.CreatedAt,
                Status = o.Items.First().Status.ToString(), // Vendor's item status
                PaymentStatus = o.PaymentStatus.ToString(),
                VendorTotal = o.Items.Sum(oi => oi.Subtotal),
                VendorCommission = o.Items.Sum(oi => oi.CommissionAmount),
                VendorPayout = o.Items.Sum(oi => oi.VendorAmount),
                ItemCount = o.Items.Count
            })
            .ToListAsync(cancellationToken);

        return orders;
    }
}

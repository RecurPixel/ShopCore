using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetMyVendorOrders;

public class GetMyVendorOrdersQueryHandler
    : IRequestHandler<GetMyVendorOrdersQuery, PaginatedList<VendorOrderDto>>
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

    public async Task<PaginatedList<VendorOrderDto>> Handle(
        GetMyVendorOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Where(o => o.Items.Any(oi => oi.VendorId == _currentUser.VendorId));

        // Apply filters
        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(o => o.Items.Any(oi =>
                oi.VendorId == _currentUser.VendorId &&
                oi.Status.ToString() == request.Status));
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= request.ToDate.Value);
        }

        // Get total count for pagination
        var totalCount = await query.CountAsync(cancellationToken);

        var vendorId = _currentUser.VendorId;

        // Get paginated orders that contain items from this vendor
        var items = await query
            .Include(o => o.User)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new VendorOrderDto
            {
                OrderId = o.Id,
                OrderNumber = o.OrderNumber,
                CustomerName = o.User.FirstName + " " + o.User.LastName,
                CustomerPhone = o.User.PhoneNumber,
                OrderDate = o.CreatedAt,
                Status = o.Items.Where(oi => oi.VendorId == vendorId).Select(oi => oi.Status.ToString()).FirstOrDefault() ?? string.Empty,
                PaymentStatus = o.PaymentStatus.ToString(),
                VendorTotal = o.Items.Where(oi => oi.VendorId == vendorId).Sum(oi => oi.Quantity * oi.UnitPrice),
                VendorCommission = o.Items.Where(oi => oi.VendorId == vendorId).Sum(oi => oi.Quantity * oi.UnitPrice * oi.CommissionRate / 100),
                VendorPayout = o.Items.Where(oi => oi.VendorId == vendorId).Sum(oi => oi.Quantity * oi.UnitPrice * (1 - oi.CommissionRate / 100)),
                ItemCount = o.Items.Count(oi => oi.VendorId == vendorId)
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<VendorOrderDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}

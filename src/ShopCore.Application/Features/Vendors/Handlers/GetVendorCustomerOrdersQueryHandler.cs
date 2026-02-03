using ShopCore.Application.Common.Models;
using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerOrders;

public class GetVendorCustomerOrdersQueryHandler : IRequestHandler<GetVendorCustomerOrdersQuery, PaginatedList<VendorOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorCustomerOrdersQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<VendorOrderDto>> Handle(
        GetVendorCustomerOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .AsNoTracking()
            .Include(o => o.Items)
            .Where(o => o.UserId == request.UserId
                && o.Items.Any(oi => oi.VendorId == _currentUser.VendorId));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new VendorOrderDto
            {
                OrderId = o.Id,
                OrderNumber = o.OrderNumber,
                OrderDate = o.CreatedAt,
                Status = o.Items.First(oi => oi.VendorId == _currentUser.VendorId).Status.ToString(),
                PaymentStatus = o.PaymentStatus.ToString(),
                VendorTotal = o.Items
                    .Where(oi => oi.VendorId == _currentUser.VendorId)
                    .Sum(oi => oi.Subtotal),
                ItemCount = o.Items.Count(oi => oi.VendorId == _currentUser.VendorId)
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<VendorOrderDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize);
    }
}

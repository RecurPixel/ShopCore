using ShopCore.Application.Common.Models;
using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomers;

public class GetVendorCustomersQueryHandler : IRequestHandler<GetVendorCustomersQuery, PaginatedList<VendorCustomerDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorCustomersQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<VendorCustomerDto>> Handle(
        GetVendorCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var vendorId = _currentUser.VendorId;

        // Get distinct customers who have orders or subscriptions with this vendor
        var customerIds = await _context.OrderItems
            .Where(oi => oi.VendorId == vendorId)
            .Select(oi => oi.Order.UserId)
            .Union(_context.Subscriptions
                .Where(s => s.VendorId == vendorId)
                .Select(s => s.UserId))
            .Distinct()
            .ToListAsync(cancellationToken);

        var totalCount = customerIds.Count;

        var pagedCustomerIds = customerIds
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var items = new List<VendorCustomerDto>();

        foreach (var customerId in pagedCustomerIds)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == customerId)
                .FirstOrDefaultAsync(cancellationToken);

            if (user != null)
            {
                var vendorOrderItems = _context.OrderItems
                    .Where(oi => oi.Order.UserId == customerId && oi.VendorId == vendorId);

                items.Add(new VendorCustomerDto
                {
                    UserId = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    TotalOrderCount = await vendorOrderItems.CountAsync(cancellationToken),
                    ActiveSubscriptionCount = await _context.Subscriptions
                        .CountAsync(s => s.UserId == customerId
                            && s.VendorId == vendorId
                            && s.Status == SubscriptionStatus.Active, cancellationToken),
                    LastOrderDate = await _context.Orders
                        .Where(o => o.UserId == customerId
                            && o.Items.Any(oi => oi.VendorId == vendorId))
                        .OrderByDescending(o => o.CreatedAt)
                        .Select(o => (DateTime?)o.CreatedAt)
                        .FirstOrDefaultAsync(cancellationToken),
                    LastDeliveryDate = await vendorOrderItems
                        .Where(oi => oi.Status == OrderItemStatus.Delivered)
                        .OrderByDescending(oi => oi.DeliveredAt)
                        .Select(oi => oi.DeliveredAt)
                        .FirstOrDefaultAsync(cancellationToken)
                });
            }
        }

        return new PaginatedList<VendorCustomerDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}
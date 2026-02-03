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
        // Get distinct customers who have orders or subscriptions with this vendor
        var customerIds = await _context.OrderItems
            .Where(oi => oi.VendorId == _currentUser.VendorId)
            .Select(oi => oi.Order.UserId)
            .Union(_context.Subscriptions
                .Where(s => s.VendorId == _currentUser.VendorId)
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
                .Select(u => new VendorCustomerDto
                {
                    UserId = u.Id,
                    Name = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (user != null)
            {
                // Get stats for this customer
                user.TotalOrders = await _context.OrderItems
                    .CountAsync(oi => oi.Order.UserId == customerId
                        && oi.VendorId == _currentUser.VendorId, cancellationToken);

                user.TotalSpent = await _context.OrderItems
                    .Where(oi => oi.Order.UserId == customerId
                        && oi.VendorId == _currentUser.VendorId
                        && oi.Order.PaymentStatus == PaymentStatus.Paid)
                    .SumAsync(oi => oi.Subtotal, cancellationToken);

                user.ActiveSubscriptions = await _context.Subscriptions
                    .CountAsync(s => s.UserId == customerId
                        && s.VendorId == _currentUser.VendorId
                        && s.Status == SubscriptionStatus.Active, cancellationToken);

                user.LastOrderDate = await _context.Orders
                    .Where(o => o.UserId == customerId
                        && o.Items.Any(oi => oi.VendorId == _currentUser.VendorId))
                    .OrderByDescending(o => o.CreatedAt)
                    .Select(o => (DateTime?)o.CreatedAt)
                    .FirstOrDefaultAsync(cancellationToken);

                items.Add(user);
            }
        }

        return new PaginatedList<VendorCustomerDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize);
    }
}
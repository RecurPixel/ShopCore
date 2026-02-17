using ShopCore.Application.Common.Models;
using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerDeliveries;

public class GetVendorCustomerDeliveriesQueryHandler : IRequestHandler<GetVendorCustomerDeliveriesQuery, PaginatedList<DeliveryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorCustomerDeliveriesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<DeliveryDto>> Handle(
        GetVendorCustomerDeliveriesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Deliveries
            .AsNoTracking()
            .Include(d => d.Subscription)
            .Include(d => d.Items)
            .Where(d => d.Subscription.UserId == request.UserId
                && d.Subscription.VendorId == _currentUser.VendorId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(d => d.ScheduledDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DeliveryDto
            {
                Id = d.Id,
                DeliveryNumber = d.DeliveryNumber,
                SubscriptionId = d.SubscriptionId,
                ScheduledDate = d.ScheduledDate,
                ActualDeliveryDate = d.ActualDeliveryDate,
                Status = d.Status.ToString(),
                PaymentStatus = d.PaymentStatus.ToString(),
                Total = d.TotalAmount,
                ItemCount = d.Items.Count
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<DeliveryDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}

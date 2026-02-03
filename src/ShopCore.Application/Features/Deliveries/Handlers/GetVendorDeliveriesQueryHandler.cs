using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Queries.GetVendorDeliveries;

public class GetVendorDeliveriesQueryHandler : IRequestHandler<GetVendorDeliveriesQuery, PaginatedList<DeliveryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorDeliveriesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<DeliveryDto>> Handle(
        GetVendorDeliveriesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Deliveries
            .AsNoTracking()
            .Include(d => d.Subscription)
                .ThenInclude(s => s.User)
            .Include(d => d.Items)
            .Where(d => d.Subscription.VendorId == _currentUser.VendorId);

        // Filter by date
        if (request.Date.HasValue)
        {
            var date = request.Date.Value.Date;
            query = query.Where(d => d.ScheduledDate.Date == date);
        }

        // Filter by status
        if (!string.IsNullOrEmpty(request.Status)
            && Enum.TryParse<DeliveryStatus>(request.Status, out var status))
        {
            query = query.Where(d => d.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(d => d.ScheduledDate)
            .ThenBy(d => d.Subscription.DeliveryAddress.Pincode)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(d => new DeliveryDto
            {
                Id = d.Id,
                DeliveryNumber = d.DeliveryNumber,
                SubscriptionId = d.SubscriptionId,
                CustomerName = d.Subscription.User.FirstName + " " + d.Subscription.User.LastName,
                CustomerPhone = d.Subscription.User.PhoneNumber,
                DeliveryAddress = d.Subscription.DeliveryAddress.AddressLine1,
                Pincode = d.Subscription.DeliveryAddress.Pincode,
                ScheduledDate = d.ScheduledDate,
                ActualDeliveryDate = d.ActualDeliveryDate,
                Status = d.Status.ToString(),
                PaymentStatus = d.PaymentStatus.ToString(),
                TotalAmount = d.TotalAmount,
                PaymentMethod = d.PaymentMethod.HasValue ? d.PaymentMethod.Value.ToString() : null,
                PaidAt = d.PaidAt,
                DeliveryPersonName = d.DeliveryPersonName,
                FailureReason = d.FailureReason,
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
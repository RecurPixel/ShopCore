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
            .Include(d => d.Subscription)
                .ThenInclude(s => s.DeliveryAddress)
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
                SubscriptionNumber = d.Subscription.SubscriptionNumber,

                // Customer Information
                CustomerName = (d.Subscription.User.FirstName + " " + d.Subscription.User.LastName).Trim(),
                CustomerPhone = d.Subscription.DeliveryAddress.PhoneNumber,

                // Delivery Address
                DeliveryAddress = d.Subscription.DeliveryAddress.AddressLine1,
                DeliveryCity = d.Subscription.DeliveryAddress.City,
                DeliveryState = d.Subscription.DeliveryAddress.State,
                Pincode = d.Subscription.DeliveryAddress.Pincode,
                Landmark = d.Subscription.DeliveryAddress.Landmark,

                // Delivery Details
                ScheduledDate = d.ScheduledDate,
                ActualDeliveryDate = d.ActualDeliveryDate,
                Status = d.Status.ToString(),
                DeliveryPersonName = d.DeliveryPersonName,
                DeliveryNotes = d.DeliveryNotes,
                FailureReason = d.FailureReason,

                // Proof of Delivery
                DeliveryPhotoUrl = d.DeliveryPhotoUrl,
                CustomerSignatureUrl = d.CustomerSignatureUrl,

                // Payment Information
                PaymentStatus = d.PaymentStatus.ToString(),
                Total = d.TotalAmount,
                PaymentMethod = d.PaymentMethod.HasValue ? d.PaymentMethod.Value.ToString() : null,
                PaymentGateway = d.PaymentGateway.ToString(),
                PaymentTransactionId = d.PaymentTransactionId,
                PaidAt = d.PaidAt,

                // Items
                ItemCount = d.Items.Count,

                // Metadata
                CreatedAt = d.CreatedAt
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
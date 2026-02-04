using ShopCore.Application.Common.Models;
using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Queries.GetSubscriptionDeliveries;

public class GetSubscriptionDeliveriesQueryHandler
    : IRequestHandler<GetSubscriptionDeliveriesQuery, PaginatedList<DeliveryDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSubscriptionDeliveriesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<DeliveryDto>> Handle(
        GetSubscriptionDeliveriesQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Deliveries
            .AsNoTracking()
            .Include(d => d.Items)
            .Include(d => d.Invoice)
            .Where(d => d.SubscriptionId == request.SubscriptionId
                     && (d.Subscription.UserId == _currentUser.UserId
                         || d.Subscription.VendorId == _currentUser.VendorId));

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
                PaymentMethod = d.PaymentMethod.ToString(),
                PaidAt = d.PaidAt,
                DeliveryPersonName = d.DeliveryPersonName,
                FailureReason = d.FailureReason,
                InvoiceId = d.InvoiceId,
                InvoiceNumber = d.Invoice != null ? d.Invoice.InvoiceNumber : null,
                ItemCount = d.Items.Count,
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
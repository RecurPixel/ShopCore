using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerSubscriptions;

public class GetVendorCustomerSubscriptionsQueryHandler : IRequestHandler<GetVendorCustomerSubscriptionsQuery, PaginatedList<VendorSubscriptionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorCustomerSubscriptionsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<VendorSubscriptionDto>> Handle(
        GetVendorCustomerSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Subscriptions
            .AsNoTracking()
            .Include(s => s.Items)
            .Where(s => s.UserId == request.UserId
                && s.VendorId == _currentUser.VendorId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new VendorSubscriptionDto
            {
                Id = s.Id,
                SubscriptionNumber = s.SubscriptionNumber,
                Frequency = s.Frequency.ToString(),
                Status = s.Status.ToString(),
                NextDeliveryDate = s.NextDeliveryDate,
                UnpaidAmount = s.UnpaidAmount,
                ItemCount = s.Items.Count,
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<VendorSubscriptionDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}
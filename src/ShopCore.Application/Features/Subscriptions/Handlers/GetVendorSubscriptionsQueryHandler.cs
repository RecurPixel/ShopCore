using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptions;

public class GetVendorSubscriptionsQueryHandler
    : IRequestHandler<GetVendorSubscriptionsQuery, PaginatedList<VendorSubscriptionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorSubscriptionsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<VendorSubscriptionDto>> Handle(
        GetVendorSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Subscriptions
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.Items)
            .Where(s => s.VendorId == _currentUser.VendorId);

        // Filter by status
        if (!string.IsNullOrEmpty(request.Status)
            && Enum.TryParse<SubscriptionStatus>(request.Status, out var status))
        {
            query = query.Where(s => s.Status == status);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new VendorSubscriptionDto
            {
                Id = s.Id,
                SubscriptionNumber = s.SubscriptionNumber,
                CustomerName = s.User.FirstName + " " + s.User.LastName,
                CustomerPhone = s.User.PhoneNumber,
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
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetMySubscriptions;

public class GetMySubscriptionsQueryHandler
    : IRequestHandler<GetMySubscriptionsQuery, List<SubscriptionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMySubscriptionsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<SubscriptionDto>> Handle(
        GetMySubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Subscriptions
            .AsNoTracking()
            .Include(s => s.Vendor)
            .Include(s => s.Items)
                .ThenInclude(si => si.Product)
                    .ThenInclude(p => p.Images)
            .Include(s => s.DeliveryAddress)
            .Where(s => s.UserId == _currentUser.UserId);

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
            .Select(s => new SubscriptionDto
            {
                Id = s.Id,
                SubscriptionNumber = s.SubscriptionNumber,
                VendorId = s.VendorId,
                VendorName = s.Vendor.BusinessName,
                Frequency = s.Frequency.ToString(),
                Status = s.Status.ToString(),
                NextDeliveryDate = s.NextDeliveryDate,
                NextBillingDate = s.NextBillingDate,
                UnpaidAmount = s.UnpaidAmount,
                DepositBalance = s.DepositBalance,
                ItemCount = s.Items.Count,
                Items = s.Items.Select(si => new SubscriptionItemDto
                {
                    ProductId = si.ProductId,
                    ProductName = si.Product.Name,
                    ProductImage = si.Product.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl,
                    Quantity = si.Quantity,
                    UnitPrice = si.UnitPrice,
                    IsRecurring = si.IsRecurring,
                    OneTimeDeliveryDate = si.OneTimeDeliveryDate
                }).ToList(),
                CreatedAt = s.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<SubscriptionDto>(
            items,
            totalCount,
            request.Page,
            request.PageSize);
    }
}

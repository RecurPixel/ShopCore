using ShopCore.Application.Addresses.DTOs;
using ShopCore.Application.Common.Models;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetAllSubscriptions;

public class GetAllSubscriptionsQueryHandler : IRequestHandler<GetAllSubscriptionsQuery, PaginatedList<SubscriptionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetAllSubscriptionsQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<SubscriptionDto>> Handle(GetAllSubscriptionsQuery request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can view all subscriptions");

        var query = _context.Subscriptions
            .AsNoTracking()
            .Include(s => s.Vendor)
            .Include(s => s.DeliveryAddress)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .AsQueryable();

        // Filter by status
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<SubscriptionStatus>(request.Status, true, out var status))
        {
            query = query.Where(s => s.Status == status);
        }

        // Filter by user
        if (request.UserId.HasValue)
        {
            query = query.Where(s => s.UserId == request.UserId.Value);
        }

        // Filter by vendor
        if (request.VendorId.HasValue)
        {
            query = query.Where(s => s.VendorId == request.VendorId.Value);
        }

        var totalCount = await query.CountAsync(ct);

        var subscriptions = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var items = subscriptions.Select(MapToDto).ToList();

        return new PaginatedList<SubscriptionDto>(items, totalCount, request.Page, request.PageSize);
    }

    private static SubscriptionDto MapToDto(Subscription subscription)
    {
        return new SubscriptionDto(
            subscription.Id,
            subscription.SubscriptionNumber,
            subscription.UserId,
            subscription.VendorId,
            subscription.Vendor.BusinessName,
            subscription.DeliveryAddressId,
            new AddressDto(
                subscription.DeliveryAddress.Id,
                subscription.DeliveryAddress.FullName,
                subscription.DeliveryAddress.PhoneNumber,
                subscription.DeliveryAddress.AddressLine1,
                subscription.DeliveryAddress.AddressLine2,
                subscription.DeliveryAddress.City,
                subscription.DeliveryAddress.State,
                subscription.DeliveryAddress.Pincode,
                subscription.DeliveryAddress.Latitude,
                subscription.DeliveryAddress.Longitude,
                subscription.DeliveryAddress.IsDefault
            ),
            subscription.Frequency,
            subscription.CustomFrequencyDays,
            subscription.StartDate,
            subscription.EndDate,
            subscription.NextDeliveryDate,
            subscription.PreferredDeliveryTime,
            subscription.BillingCycleDays,
            subscription.NextBillingDate,
            subscription.UnpaidAmount,
            subscription.CreditLimit,
            subscription.DepositAmount,
            subscription.DepositPaid,
            subscription.DepositBalance,
            subscription.Status,
            subscription.TotalDeliveries,
            subscription.CompletedDeliveries,
            subscription.FailedDeliveries,
            subscription.Items.Select(i => new SubscriptionItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Product.ImageUrl,
                i.Quantity,
                i.UnitPrice,
                i.Quantity * i.UnitPrice
            )).ToList()
        );
    }
}

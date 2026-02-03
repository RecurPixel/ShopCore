using ShopCore.Application.Addresses.DTOs;
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

        return new PaginatedList<SubscriptionDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }

    private static SubscriptionDto MapToDto(Subscription subscription)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            SubscriptionNumber = subscription.SubscriptionNumber,
            UserId = subscription.UserId,
            VendorId = subscription.VendorId,
            VendorName = subscription.Vendor.BusinessName,
            DeliveryAddressId = subscription.DeliveryAddressId,
            DeliveryAddress = new AddressDto
            {
                Id = subscription.DeliveryAddress.Id,
                FullName = subscription.DeliveryAddress.FullName,
                PhoneNumber = subscription.DeliveryAddress.PhoneNumber,
                AddressLine1 = subscription.DeliveryAddress.AddressLine1,
                AddressLine2 = subscription.DeliveryAddress.AddressLine2,
                City = subscription.DeliveryAddress.City,
                State = subscription.DeliveryAddress.State,
                Pincode = subscription.DeliveryAddress.PinCode,
                Latitude = subscription.DeliveryAddress.Latitude,
                Longitude = subscription.DeliveryAddress.Longitude,
                PlaceId = subscription.DeliveryAddress.PlaceId,
                IsDefault = subscription.DeliveryAddress.IsDefault
            },
            Frequency = subscription.Frequency.ToString(),
            CustomFrequencyDays = subscription.CustomFrequencyDays,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            NextDeliveryDate = subscription.NextDeliveryDate,
            PreferredDeliveryTime = subscription.PreferredDeliveryTime,
            BillingCycleDays = subscription.BillingCycleDays,
            NextBillingDate = subscription.NextBillingDate,
            UnpaidAmount = subscription.UnpaidAmount,
            CreditLimit = subscription.CreditLimit,
            DepositAmount = subscription.DepositAmount,
            DepositPaid = subscription.DepositPaid,
            DepositBalance = subscription.DepositBalance,
            Status = subscription.Status,
            TotalDeliveries = subscription.TotalDeliveries,
            CompletedDeliveries = subscription.CompletedDeliveries,
            FailedDeliveries = subscription.FailedDeliveries,
            Items = subscription.Items.Select(i => new SubscriptionItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductImage = i.Product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.Quantity * i.UnitPrice
            }).ToList()

        };
    }
}

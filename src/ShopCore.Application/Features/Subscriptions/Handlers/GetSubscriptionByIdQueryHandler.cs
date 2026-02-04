using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetSubscriptionById;

public class GetSubscriptionByIdQueryHandler
    : IRequestHandler<GetSubscriptionByIdQuery, SubscriptionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSubscriptionByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<SubscriptionDto> Handle(
        GetSubscriptionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.Vendor)
            .Include(s => s.DeliveryAddress)
            .Include(s => s.Items)
                .ThenInclude(si => si.Product)
                    .ThenInclude(p => p.Images)
            .Where(s => s.Id == request.SubscriptionId
                && (s.UserId == _currentUser.UserId || s.VendorId == _currentUser.VendorId))
            .Select(s => new SubscriptionDto
            {
                Id = s.Id,
                SubscriptionNumber = s.SubscriptionNumber,
                UserId = s.UserId,
                CustomerName = s.User.FirstName + " " + s.User.LastName,
                CustomerEmail = s.User.Email,
                CustomerPhone = s.User.PhoneNumber,
                VendorId = s.VendorId,
                VendorName = s.Vendor.BusinessName,
                DeliveryAddress = new SubscriptionAddressDto
                {
                    FullName = s.DeliveryAddress.FullName,
                    PhoneNumber = s.DeliveryAddress.PhoneNumber,
                    AddressLine1 = s.DeliveryAddress.AddressLine1,
                    AddressLine2 = s.DeliveryAddress.AddressLine2,
                    City = s.DeliveryAddress.City,
                    State = s.DeliveryAddress.State,
                    Pincode = s.DeliveryAddress.Pincode,
                    Landmark = s.DeliveryAddress.Landmark
                },
                Frequency = s.Frequency.ToString(),
                CustomFrequencyDays = s.CustomFrequencyDays,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                NextDeliveryDate = s.NextDeliveryDate,
                PreferredDeliveryTime = s.PreferredDeliveryTime,
                BillingCycleDays = s.BillingCycleDays,
                NextBillingDate = s.NextBillingDate,
                UnpaidAmount = s.UnpaidAmount,
                CreditLimit = s.CreditLimit,
                DepositAmount = s.DepositAmount,
                DepositPaid = s.DepositPaid,
                DepositBalance = s.DepositBalance,
                DepositPaidAt = s.DepositPaidAt,
                Status = s.Status.ToString(),
                PausedAt = s.PausedAt,
                CancelledAt = s.CancelledAt,
                CancellationReason = s.CancellationReason,
                TotalDeliveries = s.TotalDeliveries,
                CompletedDeliveries = s.CompletedDeliveries,
                FailedDeliveries = s.FailedDeliveries,
                Items = s.Items.Select(si => new SubscriptionItemDto
                {
                    Id = si.Id,
                    ProductId = si.ProductId,
                    ProductName = si.Product.Name,
                    ProductImageUrl = si.Product.Images.FirstOrDefault(i => i.IsPrimary) != null
                        ? si.Product.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl
                        : null,
                    Quantity = si.Quantity,
                    UnitPrice = si.UnitPrice,
                    LineTotal = si.Quantity * si.UnitPrice,
                    IsRecurring = si.IsRecurring,
                    OneTimeDeliveryDate = si.OneTimeDeliveryDate,
                    VendorId = s.VendorId,
                    VendorName = s.Vendor.BusinessName
                }).ToList(),
                CreatedAt = s.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (subscription == null)
            throw new NotFoundException("Subscription not found");

        return subscription;
    }
}

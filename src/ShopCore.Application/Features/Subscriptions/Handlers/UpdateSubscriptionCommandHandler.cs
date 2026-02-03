using ShopCore.Application.Addresses.DTOs;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.UpdateSubscription;

public class UpdateSubscriptionCommandHandler
    : IRequestHandler<UpdateSubscriptionCommand, SubscriptionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateSubscriptionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<SubscriptionDto> Handle(
        UpdateSubscriptionCommand request,
        CancellationToken ct)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Vendor)
            .Include(s => s.DeliveryAddress)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (subscription == null)
            throw new NotFoundException("Subscription", request.Id);

        if (subscription.UserId != _currentUser.UserId)
            throw new ForbiddenException("You can only update your own subscriptions");

        if (subscription.Status == SubscriptionStatus.Cancelled)
            throw new BadRequestException("Cannot update a cancelled subscription");

        // Update frequency settings
        subscription.Frequency = request.Frequency;
        subscription.CustomFrequencyDays = request.CustomFrequencyDays;
        subscription.PreferredDeliveryTime = request.PreferredDeliveryTime;

        // Update items if provided
        if (request.Items.Any())
        {
            // Remove existing items
            _context.SubscriptionItems.RemoveRange(subscription.Items);

            // Add new items
            foreach (var item in request.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId && p.VendorId == subscription.VendorId, ct);

                if (product == null)
                    throw new BadRequestException($"Product {item.ProductId} not found or doesn't belong to this vendor");

                subscription.Items.Add(new SubscriptionItem
                {
                    SubscriptionId = subscription.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });
            }
        }

        await _context.SaveChangesAsync(ct);

        // Reload to get updated items
        var subscriptionUpdated = await _context.Subscriptions
                .Include(s => s.Items)
                .FirstAsync(s => s.Id == subscription.Id, ct);

        return MapToDto(subscriptionUpdated);
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
            Items = subscription.Items.Select(i => new DTOs.SubscriptionItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductImage = i.Product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}

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
        await _context.Entry(subscription).Collection(s => s.Items).LoadAsync(ct);

        return MapToDto(subscription);
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
            subscription.Items.Select(i => new DTOs.SubscriptionItemDto(
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

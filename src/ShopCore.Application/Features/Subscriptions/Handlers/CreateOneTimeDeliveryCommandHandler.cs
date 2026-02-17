using ShopCore.Application.Subscriptions.Commands.CreateOneTimeDelivery;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Handlers;

public class CreateOneTimeDeliveryCommandHandler
    : IRequestHandler<CreateOneTimeDeliveryCommand, OneTimeDeliveryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CreateOneTimeDeliveryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<OneTimeDeliveryDto> Handle(
        CreateOneTimeDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.DeliveryAddressId && a.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Address), request.DeliveryAddressId);

        if (request.DeliveryDate.Date < DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException("Delivery date must be in the future");
        }

        // Get products and derive vendor from them
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .Include(p => p.Vendor)
            .Where(p => productIds.Contains(p.Id) && !p.IsDeleted)
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Count)
        {
            throw new NotFoundException("Some products were not found");
        }

        // Verify all products are from the same vendor
        var vendorIds = products.Select(p => p.VendorId).Distinct().ToList();
        if (vendorIds.Count > 1)
        {
            throw new ValidationException("All products must be from the same vendor");
        }

        var vendorId = vendorIds.First();
        var vendor = products.First().Vendor;

        // Create one-time subscription
        var subscription = new Subscription
        {
            UserId = userId,
            VendorId = vendorId,
            DeliveryAddressId = request.DeliveryAddressId,
            SubscriptionNumber = GenerateSubscriptionNumber(),
            Frequency = SubscriptionFrequency.Custom,
            StartDate = request.DeliveryDate,
            NextDeliveryDate = request.DeliveryDate,
            BillingCycleDays = 1,
            IsOneTimeDelivery = true,
            AutoCancelAfterDelivery = true,
            Status = SubscriptionStatus.Active
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync(cancellationToken);

        // Add items
        decimal totalAmount = 0;
        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            var subscriptionItem = new SubscriptionItem
            {
                SubscriptionId = subscription.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                IsRecurring = false,
                OneTimeDeliveryDate = request.DeliveryDate,
                IsDelivered = false
            };
            _context.SubscriptionItems.Add(subscriptionItem);
            totalAmount += product.Price * item.Quantity;
        }

        // Create delivery
        var delivery = new Delivery
        {
            SubscriptionId = subscription.Id,
            DeliveryNumber = GenerateDeliveryNumber(),
            ScheduledDate = request.DeliveryDate,
            Status = DeliveryStatus.Scheduled,
            PaymentStatus = request.Payment == PaymentOption.PayNow ? PaymentStatus.Pending : PaymentStatus.Unpaid,
            TotalAmount = totalAmount
        };

        _context.Deliveries.Add(delivery);
        await _context.SaveChangesAsync(cancellationToken);

        return new OneTimeDeliveryDto
        {
            SubscriptionId = subscription.Id,
            DeliveryId = delivery.Id,
            DeliveryDate = request.DeliveryDate,
            Total = totalAmount,
            PaymentStatus = delivery.PaymentStatus.ToString()
        };
    }

    private static string GenerateSubscriptionNumber()
    {
        return $"SUB-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
    }

    private static string GenerateDeliveryNumber()
    {
        return $"DEL-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
    }
}

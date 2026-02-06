using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler
    : IRequestHandler<CreateSubscriptionCommand, SubscriptionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTime _dateTime;

    public CreateSubscriptionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IDateTime dateTime)
    {
        _context = context;
        _currentUser = currentUser;
        _dateTime = dateTime;
    }

    public async Task<SubscriptionDto> Handle(CreateSubscriptionCommand request, CancellationToken ct)
    {
        // 1. Verify all products belong to same vendor
        var productIds = request.Items.Select(i => i.ProductId).ToList();
        var products = await _context.Products
            .Include(p => p.Vendor)
            .Where(p => productIds.Contains(p.Id) && !p.IsDeleted)
            .ToListAsync(ct);

        if (products.Count != productIds.Count)
            throw new ValidationException("One or more products not found");

        var vendorIds = products.Select(p => p.VendorId).Distinct().ToList();
        if (vendorIds.Count > 1)
            throw new ValidationException("All products must be from the same vendor");

        var vendorId = vendorIds.First();
        var vendor = products.First().Vendor;

        // 2. Verify all products are subscription-enabled
        if (products.Any(p => !p.IsSubscriptionAvailable))
            throw new ValidationException("Some products are not available for subscription");

        // 3. Verify address
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.DeliveryAddressId && a.UserId == _currentUser.UserId, ct);

        if (address == null)
            throw new NotFoundException("Address", request.DeliveryAddressId);

        // 4. Calculate billing cycle (if not provided)
        var billingCycleDays = request.BillingCycleDays;
        if (billingCycleDays == 0 && request.DepositAmount.HasValue)
        {
            // Auto-calculate from deposit
            var dailyTotal = request.Items.Sum(i =>
            {
                var product = products.First(p => p.Id == i.ProductId);
                var price = product.Price * i.Quantity;
                if (product.SubscriptionDiscount.HasValue)
                    price -= price * (product.SubscriptionDiscount.Value / 100);
                return price;
            });

            billingCycleDays = (int)Math.Floor(request.DepositAmount.Value / dailyTotal);
        }

        if (billingCycleDays == 0)
        {
            // Use vendor default or fallback to 30 days
            billingCycleDays = vendor.DefaultBillingCycleDays ?? 30;
        }

        // 5. Generate subscription number
        var subscriptionNumber = await GenerateSubscriptionNumberAsync(ct);

        // 6. Calculate next delivery and billing dates
        var startDate = request.StartDate.Date;
        var nextDeliveryDate = CalculateNextDeliveryDate(startDate, request.Frequency, request.CustomFrequencyDays);
        var nextBillingDate = startDate.AddDays(billingCycleDays);

        // 7. Create subscription
        var subscription = new Subscription
        {
            SubscriptionNumber = subscriptionNumber,
            UserId = _currentUser.RequiredUserId,
            VendorId = vendorId,
            DeliveryAddressId = request.DeliveryAddressId,
            Frequency = request.Frequency,
            CustomFrequencyDays = request.CustomFrequencyDays,
            StartDate = startDate,
            NextDeliveryDate = nextDeliveryDate,
            PreferredDeliveryTime = request.PreferredDeliveryTime,
            BillingCycleDays = billingCycleDays,
            NextBillingDate = nextBillingDate,
            UnpaidAmount = 0,
            CreditLimit = request.CreditLimit ?? 1200m,
            DepositAmount = request.DepositAmount,
            DepositPaid = request.DepositAmount,
            DepositBalance = request.DepositAmount,
            DepositPaidAt = request.DepositAmount.HasValue ? _dateTime.UtcNow : null,
            Status = SubscriptionStatus.Active
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync(ct); // Save to get subscription ID

        // 8. Create subscription items
        var subscriptionItems = new List<SubscriptionItem>();

        foreach (var item in request.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);

            var unitPrice = product.Price;
            decimal? discountPercentage = null;

            if (product.SubscriptionDiscount.HasValue)
            {
                discountPercentage = product.SubscriptionDiscount.Value;
            }

            subscriptionItems.Add(new SubscriptionItem
            {
                SubscriptionId = subscription.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = unitPrice,
                DiscountPercentage = discountPercentage
            });
        }

        _context.SubscriptionItems.AddRange(subscriptionItems);
        await _context.SaveChangesAsync(ct);

        return await GetSubscriptionDtoAsync(subscription.Id, ct);
    }

    private async Task<string> GenerateSubscriptionNumberAsync(CancellationToken ct)
    {
        var today = _dateTime.UtcNow;
        var prefix = $"SUB-{today:yyyyMMdd}";

        var todaySubscriptions = await _context.Subscriptions
            .Where(s => s.SubscriptionNumber.StartsWith(prefix))
            .CountAsync(ct);

        return $"{prefix}-{(todaySubscriptions + 1):D4}";
    }

    private DateTime CalculateNextDeliveryDate(DateTime startDate, SubscriptionFrequency frequency, int? customDays)
    {
        return frequency switch
        {
            SubscriptionFrequency.Daily => startDate.AddDays(1),
            SubscriptionFrequency.EveryTwoDays => startDate.AddDays(2),
            SubscriptionFrequency.Weekly => startDate.AddDays(7),
            SubscriptionFrequency.BiWeekly => startDate.AddDays(14),
            SubscriptionFrequency.Monthly => startDate.AddMonths(1),
            SubscriptionFrequency.Custom => startDate.AddDays(customDays ?? 1),
            _ => startDate.AddDays(1)
        };
    }

    private async Task<SubscriptionDto> GetSubscriptionDtoAsync(int subscriptionId, CancellationToken ct)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .Include(s => s.Vendor)
            .Include(s => s.DeliveryAddress)
            .FirstAsync(s => s.Id == subscriptionId, ct);

        return new SubscriptionDto
        {
            Id = subscription.Id,
            SubscriptionNumber = subscription.SubscriptionNumber,
            Status = subscription.Status.ToString(),
            Frequency = subscription.Frequency.ToString(),
            NextDeliveryDate = subscription.NextDeliveryDate,
            NextBillingDate = subscription.NextBillingDate,
            UnpaidAmount = subscription.UnpaidAmount,
            DepositBalance = subscription.DepositBalance,
            Items = subscription.Items.Select(i => new SubscriptionItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                DiscountPercentage = i.DiscountPercentage,
                LineTotal = i.ItemTotal
            }).ToList()
        };
    }
}

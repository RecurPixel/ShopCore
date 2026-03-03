using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Commands.SkipDelivery;

public class SkipDeliveryCommandHandler : IRequestHandler<SkipDeliveryCommand, DeliveryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly INotificationService _notificationService;

    public SkipDeliveryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        INotificationService notificationService)
    {
        _context = context;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<DeliveryDto> Handle(
        SkipDeliveryCommand request,
        CancellationToken ct)
    {
        var delivery = await _context.Deliveries
            .Include(d => d.Subscription)
            .Include(d => d.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(d => d.Id == request.Id, ct);

        if (delivery == null)
            throw new NotFoundException("Delivery", request.Id);

        // Verify user owns this delivery (either as customer or vendor)
        var isCustomer = delivery.Subscription.UserId == _currentUser.UserId;
        var isVendor = delivery.Subscription.VendorId == _currentUser.VendorId;

        if (!isCustomer && !isVendor)
            throw new ForbiddenException("You can only skip your own deliveries");

        if (delivery.Status != DeliveryStatus.Scheduled)
            throw new BadRequestException($"Cannot skip a delivery with status {delivery.Status}");

        // Check if delivery is too close to skip (e.g., within 2 hours)
        if (delivery.ScheduledDate <= DateTime.UtcNow.AddHours(2))
            throw new BadRequestException("Cannot skip a delivery that is scheduled within 2 hours");

        delivery.Status = DeliveryStatus.Skipped;
        delivery.DeliveryNotes = request.Reason;

        await _context.SaveChangesAsync(ct);

        // Notify the customer (not the vendor who may be skipping on their behalf)
        var user = await _context.Users.FindAsync(new object[] { delivery.Subscription.UserId }, ct);
        if (user != null)
            await _notificationService.SendDeliverySkippedAsync(user, delivery.Id);

        return MapToDto(delivery, request.Reason);
    }

    private static DeliveryDto MapToDto(Delivery delivery, string? skipReason)
    {
        PaymentMethod? paymentMethod = null;
        if (!string.IsNullOrEmpty(delivery.PaymentMethod.ToString()) &&
            Enum.TryParse<PaymentMethod>(delivery.PaymentMethod.ToString(), out var pm))
        {
            paymentMethod = pm;
        }

        return new DeliveryDto
        {
            Id = delivery.Id,
            SubscriptionId = delivery.SubscriptionId,
            SubscriptionNumber = delivery.Subscription.SubscriptionNumber,
            ScheduledDate = delivery.ScheduledDate,
            ActualDeliveryDate = delivery.ActualDeliveryDate,
            Status = delivery.Status.ToString(),
            FailureReason = delivery.FailureReason,
            SkipReason = skipReason,
            Total = delivery.TotalAmount,
            PaymentMethod = paymentMethod.ToString(),
            Items = delivery.Items.Select(i => new DeliveryItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Subtotal = i.Quantity * i.UnitPrice
            }).ToList()
        };
    }
}

using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Commands.CompleteDelivery;

public class CompleteDeliveryCommandHandler : IRequestHandler<CompleteDeliveryCommand, DeliveryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CompleteDeliveryCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<DeliveryDto> Handle(
        CompleteDeliveryCommand request,
        CancellationToken ct)
    {
        var delivery = await _context.Deliveries
            .Include(d => d.Subscription)
            .Include(d => d.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(d => d.Id == request.Id, ct);

        if (delivery == null)
            throw new NotFoundException("Delivery", request.Id);

        // Verify vendor owns this delivery
        if (delivery.Subscription.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only complete your own deliveries");

        if (delivery.Status == DeliveryStatus.Delivered)
            throw new BadRequestException("Delivery is already completed");

        if (delivery.Status == DeliveryStatus.Cancelled || delivery.Status == DeliveryStatus.Skipped)
            throw new BadRequestException($"Cannot complete a {delivery.Status} delivery");

        // Update item statuses if provided
        if (request.ItemStatuses?.Any() == true)
        {
            foreach (var itemStatus in request.ItemStatuses)
            {
                var item = delivery.Items.FirstOrDefault(i => i.Id == itemStatus.DeliveryItemId);
                if (item != null)
                {
                    item.Status = itemStatus.Status;
                    item.Notes = itemStatus.Notes;
                }
            }
        }

        // Complete the delivery
        delivery.Status = DeliveryStatus.Delivered;
        delivery.ActualDeliveryDate = DateTime.UtcNow;
        delivery.DeliveryPhotoUrl = request.DeliveryPhotoUrl;
        delivery.CustomerSignatureUrl = request.CustomerSignatureUrl;
        delivery.DeliveryNotes = request.DeliveryNotes;

        if (request.PaymentMethod.HasValue)
        {
            delivery.PaymentMethod = request.PaymentMethod.Value;
            delivery.PaymentTransactionId = request.PaymentTransactionId;
            delivery.PaidAt = DateTime.UtcNow;
            delivery.PaymentStatus = PaymentStatus.Paid;
        }

        // Update subscription statistics
        delivery.Subscription.CompletedDeliveries++;

        await _context.SaveChangesAsync(ct);

        return MapToDto(delivery);
    }

    private static DeliveryDto MapToDto(Delivery delivery)
    {
        PaymentMethod? paymentMethod = null;
        if (!string.IsNullOrEmpty(delivery.PaymentMethod) &&
            Enum.TryParse<PaymentMethod>(delivery.PaymentMethod, out var pm))
        {
            paymentMethod = pm;
        }

        return new DeliveryDto(
            delivery.Id,
            delivery.SubscriptionId,
            delivery.Subscription.SubscriptionNumber,
            delivery.ScheduledDate,
            delivery.ActualDeliveryDate,
            delivery.Status,
            delivery.FailureReason,
            null,
            delivery.TotalAmount,
            paymentMethod,
            delivery.PaymentTransactionId,
            delivery.Items.Select(i => new DeliveryItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Quantity,
                i.UnitPrice,
                i.Quantity * i.UnitPrice
            )).ToList()
        );
    }
}

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
                .ThenInclude(s => s.User)
            .Include(d => d.Subscription)
                .ThenInclude(s => s.DeliveryAddress)
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
        return new DeliveryDto
        {
            Id = delivery.Id,
            DeliveryNumber = delivery.DeliveryNumber,
            SubscriptionId = delivery.SubscriptionId,
            SubscriptionNumber = delivery.Subscription.SubscriptionNumber,

            // Customer Information
            CustomerName = $"{delivery.Subscription.User.FirstName} {delivery.Subscription.User.LastName}".Trim(),
            CustomerPhone = delivery.Subscription.DeliveryAddress.PhoneNumber,

            // Delivery Address
            DeliveryAddress = delivery.Subscription.DeliveryAddress.AddressLine1,
            DeliveryCity = delivery.Subscription.DeliveryAddress.City,
            DeliveryState = delivery.Subscription.DeliveryAddress.State,
            Pincode = delivery.Subscription.DeliveryAddress.Pincode,
            Landmark = delivery.Subscription.DeliveryAddress.Landmark,

            // Delivery Details
            ScheduledDate = delivery.ScheduledDate,
            ActualDeliveryDate = delivery.ActualDeliveryDate,
            Status = delivery.Status.ToString(),
            DeliveryPersonName = delivery.DeliveryPersonName,
            DeliveryNotes = delivery.DeliveryNotes,
            FailureReason = delivery.FailureReason,

            // Proof of Delivery
            DeliveryPhotoUrl = delivery.DeliveryPhotoUrl,
            CustomerSignatureUrl = delivery.CustomerSignatureUrl,

            // Payment Information
            PaymentStatus = delivery.PaymentStatus.ToString(),
            Total = delivery.TotalAmount,
            PaymentMethod = delivery.PaymentMethod?.ToString(),
            PaymentGateway = delivery.PaymentGateway.ToString(),
            PaymentTransactionId = delivery.PaymentTransactionId,
            PaidAt = delivery.PaidAt,

            // Items
            ItemCount = delivery.Items.Count,
            Items = delivery.Items.Select(i => new DeliveryItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Subtotal = i.Quantity * i.UnitPrice,
                Status = i.Status.ToString(),
                Notes = i.Notes
            }).ToList(),

            // Metadata
            CreatedAt = delivery.CreatedAt
        };
    }
}

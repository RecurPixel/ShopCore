namespace ShopCore.Application.Deliveries.Commands.MarkDeliveryFailed;

public class MarkDeliveryFailedCommandHandler : IRequestHandler<MarkDeliveryFailedCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MarkDeliveryFailedCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(MarkDeliveryFailedCommand request, CancellationToken ct)
    {
        var delivery = await _context.Deliveries
            .Include(d => d.Subscription)
            .FirstOrDefaultAsync(d => d.Id == request.DeliveryId, ct);

        if (delivery == null)
            throw new NotFoundException("Delivery", request.DeliveryId);

        // Verify vendor owns this delivery's subscription
        if (delivery.Subscription.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only update your own deliveries");

        // Verify delivery is in a valid state to be marked as failed
        if (delivery.Status != DeliveryStatus.Scheduled && delivery.Status != DeliveryStatus.OutForDelivery)
            throw new BadRequestException($"Cannot mark delivery as failed. Current status: {delivery.Status}");

        // Update delivery status
        delivery.Status = DeliveryStatus.Failed;
        delivery.FailureReason = request.Reason;
        delivery.ActualDeliveryDate = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
    }
}

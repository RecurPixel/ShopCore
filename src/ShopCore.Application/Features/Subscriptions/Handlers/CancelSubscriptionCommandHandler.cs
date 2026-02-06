namespace ShopCore.Application.Subscriptions.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler : IRequestHandler<CancelSubscriptionCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CancelSubscriptionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(
        CancelSubscriptionCommand request,
        CancellationToken ct)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Deliveries)
            .FirstOrDefaultAsync(s => s.Id == request.SubscriptionId, ct);

        if (subscription == null)
            throw new NotFoundException("Subscription", request.SubscriptionId);

        if (subscription.UserId != _currentUser.UserId && _currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("You can only cancel your own subscriptions");

        if (subscription.Status == SubscriptionStatus.Cancelled)
            throw new BadRequestException("Subscription is already cancelled");

        subscription.Status = SubscriptionStatus.Cancelled;
        subscription.CancelledAt = DateTime.UtcNow;
        subscription.EndDate = DateTime.UtcNow;

        // Cancel pending deliveries
        var pendingDeliveries = subscription.Deliveries
            .Where(d => d.Status == DeliveryStatus.Scheduled || d.Status == DeliveryStatus.OutForDelivery);

        foreach (var delivery in pendingDeliveries)
        {
            delivery.Status = DeliveryStatus.Cancelled;
        }

        await _context.SaveChangesAsync(ct);
    }
}

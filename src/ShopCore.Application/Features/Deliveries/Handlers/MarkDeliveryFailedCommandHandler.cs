namespace ShopCore.Application.Deliveries.Commands.MarkDeliveryFailed;

public class MarkDeliveryFailedCommandHandler : IRequestHandler<MarkDeliveryFailedCommand>
{
    public Task Handle(
        MarkDeliveryFailedCommand request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get delivery by id
        // 2. Verify current user is delivery person
        // 3. Update delivery status to 'failed'
        // 4. Record failure reason and timestamp
        // 5. Upload failure photo if provided
        // 6. Trigger retry or refund logic
        // 7. Notify customer and vendor
        return Task.CompletedTask;
    }
}

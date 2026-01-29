using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Commands.FailDelivery;

public class FailDeliveryCommandHandler : IRequestHandler<FailDeliveryCommand, DeliveryDto>
{
    public Task<DeliveryDto> Handle(
        FailDeliveryCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        // 1. Get delivery by id
        // 2. Verify current user is delivery person
        // 3. Check delivery is in pending/in-transit status
        // 4. Update delivery status to failed
        // 5. Record failure reason and timestamp
        // 6. Upload failure photo if provided
        // 7. Trigger retry/refund workflow
        // 8. Notify customer and vendor
        // 9. Map and return updated DeliveryDto
        return Task.FromResult(new DeliveryDto());
    }
}

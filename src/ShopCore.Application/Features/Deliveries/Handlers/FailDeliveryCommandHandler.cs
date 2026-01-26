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
        return Task.FromResult(new DeliveryDto());
    }
}

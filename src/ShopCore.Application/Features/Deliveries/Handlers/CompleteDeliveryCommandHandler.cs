using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Commands.CompleteDelivery;

public class CompleteDeliveryCommandHandler : IRequestHandler<CompleteDeliveryCommand, DeliveryDto>
{
    public Task<DeliveryDto> Handle(
        CompleteDeliveryCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new DeliveryDto());
    }
}

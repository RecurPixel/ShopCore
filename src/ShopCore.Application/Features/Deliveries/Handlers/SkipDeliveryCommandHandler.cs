using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Commands.SkipDelivery;

public class SkipDeliveryCommandHandler : IRequestHandler<SkipDeliveryCommand, DeliveryDto>
{
    public Task<DeliveryDto> Handle(
        SkipDeliveryCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new DeliveryDto());
    }
}

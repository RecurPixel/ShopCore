namespace ShopCore.Application.Deliveries.Commands.SkipDelivery;

public class SkipDeliveryCommandHandler : IRequestHandler<SkipDeliveryCommand>
{
    public Task Handle(SkipDeliveryCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}

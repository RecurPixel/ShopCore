namespace ShopCore.Application.Deliveries.Commands.FailDelivery;

public class FailDeliveryCommandHandler : IRequestHandler<FailDeliveryCommand>
{
    public Task Handle(FailDeliveryCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}

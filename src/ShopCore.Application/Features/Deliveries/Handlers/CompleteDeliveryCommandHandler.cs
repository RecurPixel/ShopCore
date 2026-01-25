namespace ShopCore.Application.Deliveries.Commands.CompleteDelivery;

public class CompleteDeliveryCommandHandler : IRequestHandler<CompleteDeliveryCommand>
{
    public Task Handle(CompleteDeliveryCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}

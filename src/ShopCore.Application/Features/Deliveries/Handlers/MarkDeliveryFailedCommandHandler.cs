namespace ShopCore.Application.Deliveries.Commands.MarkDeliveryFailed;

public class MarkDeliveryFailedCommandHandler : IRequestHandler<MarkDeliveryFailedCommand>
{
    public Task Handle(
        MarkDeliveryFailedCommand request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        throw new NotImplementedException();
    }
}

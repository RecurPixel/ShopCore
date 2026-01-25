namespace ShopCore.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand>
{
    public Task Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}

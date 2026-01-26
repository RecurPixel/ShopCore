using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.FromResult(new OrderDto());
    }
}

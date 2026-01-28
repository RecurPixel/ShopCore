using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto>
{
    public Task<OrderDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        throw new NotImplementedException();
    }
}

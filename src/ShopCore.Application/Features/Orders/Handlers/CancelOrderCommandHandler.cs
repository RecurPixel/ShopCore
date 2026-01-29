using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, OrderDto>
{
    public Task<OrderDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Find order by id
        // 3. Verify user owns the order
        // 4. Check order status (can't cancel if already shipped/delivered)
        // 5. Process refunds to customer
        // 6. Notify vendors of cancellation
        // 7. Update order status to cancelled
        // 8. Save changes and return updated OrderDto
        return Task.FromResult(new OrderDto());
    }
}

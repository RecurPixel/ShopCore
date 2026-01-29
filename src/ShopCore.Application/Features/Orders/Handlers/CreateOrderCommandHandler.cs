using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    public Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Get user's cart and validate it's not empty
        // 3. Validate all cart items still available
        // 4. Validate delivery address
        // 5. Calculate order total, taxes, shipping
        // 6. Apply coupon discount if present
        // 7. Create Order entity with items
        // 8. Clear cart after order creation
        // 9. Create payment intent
        // 10. Save to database and map return OrderDto
        return Task.FromResult(new OrderDto());
    }
}

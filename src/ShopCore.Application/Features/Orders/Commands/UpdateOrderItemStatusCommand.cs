namespace ShopCore.Application.Orders.Commands.UpdateOrderItemStatus;

public record UpdateOrderItemStatusCommand(
    int OrderItemId,
    OrderItemStatus NewStatus,
    string? Notes,
    string? TrackingNumber
) : IRequest<bool>;

using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Commands.CancelOrderItem;

public record CancelOrderItemCommand(
    int OrderItemId,
    string Reason
) : IRequest<CancellationResultDto>;

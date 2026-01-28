using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Commands.CancelOrder;

public record CancelOrderCommand(int OrderId, string? Reason = null) : IRequest<OrderDto>;

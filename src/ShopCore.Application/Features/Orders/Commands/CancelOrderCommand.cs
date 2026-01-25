using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Commands.CancelOrder;

public record CancelOrderCommand(int id) : IRequest<OrderDto>;

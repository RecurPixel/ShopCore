using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Queries.GetMyOrders;

public record GetMyOrdersQuery : IRequest<List<OrderDto>>;

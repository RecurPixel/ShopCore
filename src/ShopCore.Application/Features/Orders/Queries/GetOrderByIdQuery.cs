using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(int Id) : IRequest<OrderDetailDto>;

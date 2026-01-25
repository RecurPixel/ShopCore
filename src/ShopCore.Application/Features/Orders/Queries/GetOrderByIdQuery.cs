using ShopCore.Application.Features.Orders.DTOs;

namespace ShopCore.Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(int Id) : IRequest<OrderDetailDto>;

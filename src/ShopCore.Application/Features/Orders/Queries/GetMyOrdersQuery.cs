using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Queries.GetMyOrders;

public record GetMyOrdersQuery(
    string? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<OrderDto>>;

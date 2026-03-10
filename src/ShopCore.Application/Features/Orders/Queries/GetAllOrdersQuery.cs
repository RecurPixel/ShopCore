using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Queries.GetAllOrders;

public record GetAllOrdersQuery(
    string? Status = null,
    int? UserId = null,
    int? VendorId = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<OrderDto>>;

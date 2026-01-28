using ShopCore.Application.Common.Models;
using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, PaginatedList<OrderDto>>
{
    public Task<PaginatedList<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerOrders;

public record GetVendorCustomerOrdersQuery(
    int UserId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<VendorOrderDto>>;

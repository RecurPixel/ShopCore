using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetMyVendorOrders;

public record GetMyVendorOrdersQuery : IRequest<List<VendorOrderDto>>;

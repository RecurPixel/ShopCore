using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorOrderItems;

public record GetVendorOrderItemsQuery(int OrderId) : IRequest<List<VendorOrderItemDto>>;

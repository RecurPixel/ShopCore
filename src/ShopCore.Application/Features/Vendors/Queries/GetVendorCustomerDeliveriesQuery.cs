using ShopCore.Application.Common.Models;
using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerDeliveries;

public record GetVendorCustomerDeliveriesQuery(
    int UserId,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<DeliveryDto>>;

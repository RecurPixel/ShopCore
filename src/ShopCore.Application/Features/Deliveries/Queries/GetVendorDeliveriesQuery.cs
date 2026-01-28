using ShopCore.Application.Common.Models;
using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Queries.GetVendorDeliveries;

public record GetVendorDeliveriesQuery(
    DateTime? Date = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<DeliveryDto>>;

using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Commands.UpdateVendorOrderStatus;

public record UpdateVendorOrderStatusCommand(int OrderId, OrderStatus Status, string? Notes)
    : IRequest<VendorOrderDto>;

using ShopCore.Application.Orders.DTOs;

namespace ShopCore.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(
    int AddressId,
    PaymentMethod PaymentMethod,
    string? CouponCode,
    string? CustomerNotes
) : IRequest<OrderDto>;

using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.CreateOneTimeDelivery;

public record CreateOneTimeDeliveryCommand(
    int VendorId,
    int DeliveryAddressId,
    List<OrderItemInput> Items,
    DateTime DeliveryDate,
    PaymentOption Payment
) : IRequest<OneTimeDeliveryDto>;

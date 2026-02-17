using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.CreateOneTimeDelivery;

// Note: VendorId is derived from the products - all products must be from the same vendor
public record CreateOneTimeDeliveryCommand(
    int DeliveryAddressId,
    List<OrderItemInput> Items,
    DateTime DeliveryDate,
    PaymentOption Payment
) : IRequest<OneTimeDeliveryDto>;

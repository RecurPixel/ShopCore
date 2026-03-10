using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.CreateOrderPaymentIntent;

/// <summary>
/// Command to create a payment intent for an order
/// </summary>
/// <param name="OrderId">The order ID to create payment for</param>
/// <param name="Gateway">Optional payment gateway to use. If null, uses default gateway.</param>
public record CreateOrderPaymentIntentCommand(
    int OrderId,
    PaymentGateway? Gateway = null
) : IRequest<PaymentIntentDto>;

using ShopCore.Application.Payments.DTOs;

namespace ShopCore.Application.Payments.Commands.CreateOrderPaymentIntent;

public record CreateOrderPaymentIntentCommand(int OrderId) : IRequest<PaymentIntentDto>;

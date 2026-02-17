using ShopCore.Application.Payments.DTOs;
using ShopCore.Domain.Enums;

namespace ShopCore.Application.Payments.Commands.ConfirmPayment;

/// <summary>
/// Command to confirm a payment after checkout completion.
/// Frontend sends the gateway callback data for server-side verification.
/// </summary>
/// <param name="Gateway">The payment gateway used</param>
/// <param name="GatewayOrderId">Gateway order/intent ID (e.g., razorpay_order_id, payment_intent_id)</param>
/// <param name="GatewayPaymentId">Gateway payment ID (e.g., razorpay_payment_id)</param>
/// <param name="Signature">Optional signature for verification (required for Razorpay)</param>
/// <param name="AdditionalData">Optional additional gateway-specific data</param>
public record ConfirmPaymentCommand(
    PaymentGateway Gateway,
    string GatewayOrderId,
    string GatewayPaymentId,
    string? Signature = null,
    IDictionary<string, string>? AdditionalData = null
) : IRequest<PaymentConfirmationDto>;

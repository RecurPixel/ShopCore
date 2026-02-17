using ShopCore.Application.Common.Models;
using ShopCore.Domain.Enums;

namespace ShopCore.Application.Common.Interfaces;

/// <summary>
/// Generic payment gateway interface - provider agnostic.
/// Each payment provider (Razorpay, Stripe, PayPal, etc.) implements this interface.
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// The gateway type this implementation handles
    /// </summary>
    PaymentGateway GatewayType { get; }

    /// <summary>
    /// Display name for the UI (e.g., "Razorpay", "Stripe", "Cash on Delivery")
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Description for the UI (e.g., "Pay with UPI, Cards, Net Banking")
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Supported payment methods for this gateway
    /// </summary>
    IReadOnlyCollection<PaymentMethod> SupportedMethods { get; }

    /// <summary>
    /// Creates a payment intent/order with the gateway
    /// </summary>
    Task<CreatePaymentResult> CreatePaymentAsync(CreatePaymentRequest request, CancellationToken ct = default);

    /// <summary>
    /// Verifies payment completion (signature verification or status check)
    /// </summary>
    Task<VerifyPaymentResult> VerifyPaymentAsync(VerifyPaymentRequest request, CancellationToken ct = default);

    /// <summary>
    /// Gets current payment status from the gateway
    /// </summary>
    Task<PaymentStatusResult> GetPaymentStatusAsync(string gatewayPaymentId, CancellationToken ct = default);

    /// <summary>
    /// Creates a refund via the gateway
    /// </summary>
    Task<RefundResult> CreateRefundAsync(CreateRefundRequest request, CancellationToken ct = default);

    /// <summary>
    /// Verifies webhook signature from the gateway
    /// </summary>
    bool VerifyWebhookSignature(string payload, string signature);

    /// <summary>
    /// Parses webhook payload to a generic event
    /// </summary>
    PaymentWebhookEvent ParseWebhookPayload(string payload);
}

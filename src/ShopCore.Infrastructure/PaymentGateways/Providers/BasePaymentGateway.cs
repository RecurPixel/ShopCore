using ShopCore.Application.Common.Interfaces;
using ShopCore.Application.Common.Models;
using ShopCore.Domain.Enums;

namespace ShopCore.Infrastructure.PaymentGateways.Providers;

/// <summary>
/// Base class for payment gateway implementations with common helper methods
/// </summary>
public abstract class BasePaymentGateway : IPaymentGateway
{
    public abstract PaymentGateway GatewayType { get; }
    public abstract string DisplayName { get; }
    public abstract string Description { get; }
    public abstract IReadOnlyCollection<PaymentMethod> SupportedMethods { get; }

    public abstract Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request, CancellationToken ct = default);

    public abstract Task<VerifyPaymentResult> VerifyPaymentAsync(
        VerifyPaymentRequest request, CancellationToken ct = default);

    public abstract Task<PaymentStatusResult> GetPaymentStatusAsync(
        string gatewayPaymentId, CancellationToken ct = default);

    public abstract Task<RefundResult> CreateRefundAsync(
        CreateRefundRequest request, CancellationToken ct = default);

    public abstract bool VerifyWebhookSignature(string payload, string signature);

    public abstract PaymentWebhookEvent ParseWebhookPayload(string payload);

    /// <summary>
    /// Parse payment method string to enum
    /// </summary>
    protected static PaymentMethod ParsePaymentMethod(string? method) =>
        method?.ToLower() switch
        {
            "card" => PaymentMethod.Card,
            "upi" => PaymentMethod.UPI,
            "netbanking" => PaymentMethod.NetBanking,
            "wallet" => PaymentMethod.Wallet,
            "cod" or "cash" => PaymentMethod.CashOnDelivery,
            _ => PaymentMethod.Online
        };

    /// <summary>
    /// Generate a unique order/transaction ID
    /// </summary>
    protected static string GenerateOrderId(string prefix) =>
        $"{prefix}_{Guid.NewGuid():N}"[..24];
}

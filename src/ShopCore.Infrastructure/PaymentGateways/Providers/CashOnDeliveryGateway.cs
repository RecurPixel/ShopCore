using ShopCore.Application.Common.Models;
using ShopCore.Domain.Enums;

namespace ShopCore.Infrastructure.PaymentGateways.Providers;

/// <summary>
/// Cash on Delivery payment gateway implementation.
/// This is a special "gateway" that doesn't interact with any external service.
/// </summary>
public class CashOnDeliveryGateway : BasePaymentGateway
{
    public override PaymentGateway GatewayType => PaymentGateway.Manual;
    public override string DisplayName => "Cash on Delivery";
    public override string Description => "Pay in cash when your order is delivered";

    public override IReadOnlyCollection<PaymentMethod> SupportedMethods =>
        new[] { PaymentMethod.CashOnDelivery };

    public override Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request, CancellationToken ct = default)
    {
        // COD doesn't create an external payment - just generates a reference ID
        var orderId = $"COD_{request.ReferenceId}_{DateTime.UtcNow.Ticks}";

        return Task.FromResult(new CreatePaymentResult
        {
            Success = true,
            GatewayOrderId = orderId,
            Amount = request.Amount,
            Currency = request.Currency,
            Gateway = PaymentGateway.Manual,
            ClientData = new Dictionary<string, string>
            {
                ["type"] = "cod",
                ["message"] = "Pay in cash at delivery",
                ["order_id"] = orderId
            },
            ReferenceId = request.ReferenceId,
            ReferenceType = request.ReferenceType
        });
    }

    public override Task<VerifyPaymentResult> VerifyPaymentAsync(
        VerifyPaymentRequest request, CancellationToken ct = default)
    {
        // COD is always valid at creation - payment status stays pending until delivery
        return Task.FromResult(new VerifyPaymentResult
        {
            IsValid = true,
            Status = PaymentStatus.Pending,
            GatewayPaymentId = request.GatewayOrderId,
            Method = PaymentMethod.CashOnDelivery
        });
    }

    public override Task<PaymentStatusResult> GetPaymentStatusAsync(
        string gatewayPaymentId, CancellationToken ct = default)
    {
        // COD status is managed internally, not via gateway
        return Task.FromResult(new PaymentStatusResult
        {
            Success = true,
            GatewayPaymentId = gatewayPaymentId,
            Status = PaymentStatus.Pending,
            Amount = 0,
            Method = PaymentMethod.CashOnDelivery
        });
    }

    public override Task<RefundResult> CreateRefundAsync(
        CreateRefundRequest request, CancellationToken ct = default)
    {
        // COD doesn't support automated refunds
        return Task.FromResult(new RefundResult
        {
            Success = false,
            ErrorMessage = "Cash on Delivery orders cannot be refunded through this system. Please process refund manually.",
            RefundId = string.Empty,
            GatewayPaymentId = request.GatewayPaymentId,
            Amount = request.Amount,
            Status = RefundStatus.Failed,
            CreatedAt = DateTime.UtcNow
        });
    }

    public override bool VerifyWebhookSignature(string payload, string signature)
    {
        // COD doesn't have webhooks
        return false;
    }

    public override PaymentWebhookEvent ParseWebhookPayload(string payload)
    {
        throw new NotSupportedException("Cash on Delivery does not support webhooks");
    }
}

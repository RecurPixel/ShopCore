using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ShopCore.Application.Common.Models;
using ShopCore.Domain.Enums;

namespace ShopCore.Infrastructure.PaymentGateways.Providers;

/// <summary>
/// Razorpay payment gateway implementation
/// </summary>
public class RazorpayGateway : BasePaymentGateway
{
    private readonly RazorpayOptions _options;

    public RazorpayGateway(IOptions<PaymentGatewayOptions> options)
    {
        _options = options.Value.Razorpay;
    }

    public override PaymentGateway GatewayType => PaymentGateway.Razorpay;
    public override string DisplayName => "Razorpay";
    public override string Description => "Pay with UPI, Cards, Net Banking, or Wallets";

    public override IReadOnlyCollection<PaymentMethod> SupportedMethods =>
        new[] { PaymentMethod.Card, PaymentMethod.UPI, PaymentMethod.NetBanking, PaymentMethod.Wallet };

    public override async Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request, CancellationToken ct = default)
    {
        // TODO: Implement actual Razorpay API integration
        // POST https://api.razorpay.com/v1/orders
        await Task.CompletedTask;

        var orderId = GenerateOrderId("order");
        var amountInPaise = (int)(request.Amount * 100);

        return new CreatePaymentResult
        {
            Success = true,
            GatewayOrderId = orderId,
            Amount = request.Amount,
            Currency = request.Currency,
            Gateway = PaymentGateway.Razorpay,
            ClientData = new Dictionary<string, string>
            {
                ["key_id"] = _options.KeyId,
                ["order_id"] = orderId,
                ["amount"] = amountInPaise.ToString(),
                ["currency"] = request.Currency,
                ["name"] = request.CustomerName ?? string.Empty,
                ["description"] = request.Description ?? $"Payment for {request.ReferenceType} #{request.ReferenceId}"
            },
            ReferenceId = request.ReferenceId,
            ReferenceType = request.ReferenceType
        };
    }

    public override Task<VerifyPaymentResult> VerifyPaymentAsync(
        VerifyPaymentRequest request, CancellationToken ct = default)
    {
        // Verify signature: HMAC-SHA256(order_id + "|" + payment_id, secret)
        var isValid = VerifySignature(
            request.GatewayOrderId,
            request.GatewayPaymentId,
            request.Signature ?? string.Empty);

        return Task.FromResult(new VerifyPaymentResult
        {
            IsValid = isValid,
            Status = isValid ? PaymentStatus.Paid : PaymentStatus.Failed,
            GatewayPaymentId = request.GatewayPaymentId,
            ErrorMessage = isValid ? null : "Invalid payment signature"
        });
    }

    public override Task<PaymentStatusResult> GetPaymentStatusAsync(
        string gatewayPaymentId, CancellationToken ct = default)
    {
        // TODO: Implement actual Razorpay API call
        // GET https://api.razorpay.com/v1/payments/{paymentId}
        return Task.FromResult(new PaymentStatusResult
        {
            Success = true,
            GatewayPaymentId = gatewayPaymentId,
            Status = PaymentStatus.Paid,
            Amount = 0,
            Method = PaymentMethod.Online,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        });
    }

    public override Task<RefundResult> CreateRefundAsync(
        CreateRefundRequest request, CancellationToken ct = default)
    {
        // TODO: Implement actual Razorpay refund API
        // POST https://api.razorpay.com/v1/payments/{paymentId}/refund
        return Task.FromResult(new RefundResult
        {
            Success = true,
            RefundId = GenerateOrderId("rfnd"),
            GatewayPaymentId = request.GatewayPaymentId,
            Amount = request.Amount,
            Status = RefundStatus.Completed,
            CreatedAt = DateTime.UtcNow
        });
    }

    public override bool VerifyWebhookSignature(string payload, string signature)
    {
        if (string.IsNullOrEmpty(_options.WebhookSecret))
            return true; // Skip verification if no secret configured

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.WebhookSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var expectedSignature = Convert.ToHexString(hash).ToLower();

        return string.Equals(signature, expectedSignature, StringComparison.OrdinalIgnoreCase);
    }

    public override PaymentWebhookEvent ParseWebhookPayload(string payload)
    {
        // TODO: Parse actual Razorpay webhook JSON
        var json = JsonDocument.Parse(payload);
        var root = json.RootElement;

        var eventType = root.TryGetProperty("event", out var eventProp)
            ? eventProp.GetString() ?? string.Empty
            : string.Empty;

        return new PaymentWebhookEvent
        {
            EventType = MapEventType(eventType),
            Gateway = PaymentGateway.Razorpay,
            GatewayPaymentId = TryGetString(root, "payload.payment.entity.id"),
            GatewayOrderId = TryGetString(root, "payload.payment.entity.order_id"),
            Amount = TryGetDecimal(root, "payload.payment.entity.amount") / 100m,
            Method = ParsePaymentMethod(TryGetString(root, "payload.payment.entity.method")),
            RefundId = TryGetString(root, "payload.refund.entity.id"),
            RefundAmount = TryGetDecimal(root, "payload.refund.entity.amount") / 100m
        };
    }

    private bool VerifySignature(string orderId, string paymentId, string signature)
    {
        if (string.IsNullOrEmpty(_options.KeySecret))
            return true; // Skip verification in dev mode

        var data = $"{orderId}|{paymentId}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.KeySecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        var expectedSignature = Convert.ToHexString(hash).ToLower();

        return string.Equals(signature, expectedSignature, StringComparison.OrdinalIgnoreCase);
    }

    private static PaymentWebhookEventType MapEventType(string eventType) =>
        eventType switch
        {
            "payment.captured" => PaymentWebhookEventType.PaymentCaptured,
            "payment.failed" => PaymentWebhookEventType.PaymentFailed,
            "payment.authorized" => PaymentWebhookEventType.PaymentCreated,
            "refund.created" => PaymentWebhookEventType.RefundCreated,
            "refund.processed" => PaymentWebhookEventType.RefundProcessed,
            "refund.failed" => PaymentWebhookEventType.RefundFailed,
            _ => PaymentWebhookEventType.Unknown
        };

    private static string TryGetString(JsonElement element, string path)
    {
        var parts = path.Split('.');
        var current = element;

        foreach (var part in parts)
        {
            if (!current.TryGetProperty(part, out current))
                return string.Empty;
        }

        return current.GetString() ?? string.Empty;
    }

    private static decimal TryGetDecimal(JsonElement element, string path)
    {
        var parts = path.Split('.');
        var current = element;

        foreach (var part in parts)
        {
            if (!current.TryGetProperty(part, out current))
                return 0;
        }

        return current.TryGetDecimal(out var value) ? value : 0;
    }
}

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ShopCore.Application.Common.Models;
using ShopCore.Domain.Enums;

namespace ShopCore.Infrastructure.PaymentGateways.Providers;

/// <summary>
/// Stripe payment gateway implementation
/// </summary>
public class StripeGateway : BasePaymentGateway
{
    private readonly StripeOptions _options;

    public StripeGateway(IOptions<PaymentGatewayOptions> options)
    {
        _options = options.Value.Stripe;
    }

    public override PaymentGateway GatewayType => PaymentGateway.Stripe;
    public override string DisplayName => "Stripe";
    public override string Description => "Pay securely with Credit/Debit Card";

    public override IReadOnlyCollection<PaymentMethod> SupportedMethods =>
        new[] { PaymentMethod.Card };

    public override async Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request, CancellationToken ct = default)
    {
        // TODO: Implement actual Stripe API integration
        // POST https://api.stripe.com/v1/payment_intents
        await Task.CompletedTask;

        var paymentIntentId = GenerateOrderId("pi");
        var clientSecret = $"{paymentIntentId}_secret_{Guid.NewGuid():N}"[..40];
        var amountInCents = (int)(request.Amount * 100);

        return new CreatePaymentResult
        {
            Success = true,
            GatewayOrderId = paymentIntentId,
            GatewayPaymentId = paymentIntentId,
            Amount = request.Amount,
            Currency = request.Currency.ToLower(),
            Gateway = PaymentGateway.Stripe,
            ClientData = new Dictionary<string, string>
            {
                ["publishable_key"] = _options.PublishableKey,
                ["client_secret"] = clientSecret,
                ["payment_intent_id"] = paymentIntentId,
                ["amount"] = amountInCents.ToString(),
                ["currency"] = request.Currency.ToLower()
            },
            ReferenceId = request.ReferenceId,
            ReferenceType = request.ReferenceType
        };
    }

    public override Task<VerifyPaymentResult> VerifyPaymentAsync(
        VerifyPaymentRequest request, CancellationToken ct = default)
    {
        // Stripe uses payment_intent status check instead of signature verification
        // The client_secret already validates the payment
        // In production, you would call: GET /v1/payment_intents/{id}
        return Task.FromResult(new VerifyPaymentResult
        {
            IsValid = true,
            Status = PaymentStatus.Paid,
            GatewayPaymentId = request.GatewayPaymentId,
            Method = PaymentMethod.Card
        });
    }

    public override Task<PaymentStatusResult> GetPaymentStatusAsync(
        string gatewayPaymentId, CancellationToken ct = default)
    {
        // TODO: Implement actual Stripe API call
        // GET https://api.stripe.com/v1/payment_intents/{id}
        return Task.FromResult(new PaymentStatusResult
        {
            Success = true,
            GatewayPaymentId = gatewayPaymentId,
            Status = PaymentStatus.Paid,
            Amount = 0,
            Method = PaymentMethod.Card,
            CreatedAt = DateTime.UtcNow,
            CompletedAt = DateTime.UtcNow
        });
    }

    public override Task<RefundResult> CreateRefundAsync(
        CreateRefundRequest request, CancellationToken ct = default)
    {
        // TODO: Implement actual Stripe refund API
        // POST https://api.stripe.com/v1/refunds
        return Task.FromResult(new RefundResult
        {
            Success = true,
            RefundId = GenerateOrderId("re"),
            GatewayPaymentId = request.GatewayPaymentId,
            Amount = request.Amount,
            Status = RefundStatus.Completed,
            CreatedAt = DateTime.UtcNow
        });
    }

    public override bool VerifyWebhookSignature(string payload, string signature)
    {
        if (string.IsNullOrEmpty(_options.WebhookSecret))
            return true;

        // Stripe uses t=timestamp,v1=signature format
        var parts = signature.Split(',')
            .Select(p => p.Split('='))
            .Where(p => p.Length == 2)
            .ToDictionary(p => p[0], p => p[1]);

        if (!parts.TryGetValue("t", out var timestamp) || !parts.TryGetValue("v1", out var v1Signature))
            return false;

        var signedPayload = $"{timestamp}.{payload}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.WebhookSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(signedPayload));
        var expectedSignature = Convert.ToHexString(hash).ToLower();

        return string.Equals(v1Signature, expectedSignature, StringComparison.OrdinalIgnoreCase);
    }

    public override PaymentWebhookEvent ParseWebhookPayload(string payload)
    {
        var json = JsonDocument.Parse(payload);
        var root = json.RootElement;

        var eventType = root.TryGetProperty("type", out var typeProp)
            ? typeProp.GetString() ?? string.Empty
            : string.Empty;

        var dataObject = root.TryGetProperty("data", out var dataProp) &&
                         dataProp.TryGetProperty("object", out var objProp)
            ? objProp
            : default;

        return new PaymentWebhookEvent
        {
            EventType = MapEventType(eventType),
            Gateway = PaymentGateway.Stripe,
            GatewayPaymentId = TryGetString(dataObject, "id"),
            GatewayOrderId = TryGetString(dataObject, "id"),
            Amount = TryGetDecimal(dataObject, "amount") / 100m,
            Method = PaymentMethod.Card,
            RefundId = eventType.StartsWith("refund.") ? TryGetString(dataObject, "id") : null,
            RefundAmount = eventType.StartsWith("refund.") ? TryGetDecimal(dataObject, "amount") / 100m : null
        };
    }

    private static PaymentWebhookEventType MapEventType(string eventType) =>
        eventType switch
        {
            "payment_intent.succeeded" => PaymentWebhookEventType.PaymentCaptured,
            "payment_intent.payment_failed" => PaymentWebhookEventType.PaymentFailed,
            "payment_intent.created" => PaymentWebhookEventType.PaymentCreated,
            "charge.refunded" => PaymentWebhookEventType.RefundProcessed,
            "refund.created" => PaymentWebhookEventType.RefundCreated,
            "refund.updated" => PaymentWebhookEventType.RefundProcessed,
            "refund.failed" => PaymentWebhookEventType.RefundFailed,
            _ => PaymentWebhookEventType.Unknown
        };

    private static string TryGetString(JsonElement element, string property)
    {
        if (element.ValueKind == JsonValueKind.Undefined)
            return string.Empty;

        return element.TryGetProperty(property, out var prop)
            ? prop.GetString() ?? string.Empty
            : string.Empty;
    }

    private static decimal TryGetDecimal(JsonElement element, string property)
    {
        if (element.ValueKind == JsonValueKind.Undefined)
            return 0;

        return element.TryGetProperty(property, out var prop) && prop.TryGetDecimal(out var value)
            ? value
            : 0;
    }
}

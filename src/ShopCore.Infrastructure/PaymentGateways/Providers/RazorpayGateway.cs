using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using ShopCore.Application.Common.Models;
using ShopCore.Domain.Enums;

namespace ShopCore.Infrastructure.PaymentGateways.Providers;

/// <summary>
/// Razorpay payment gateway implementation using REST API
/// </summary>
public class RazorpayGateway : BasePaymentGateway
{
    private readonly RazorpayOptions _options;
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.razorpay.com/v1";

    public RazorpayGateway(IOptions<PaymentGatewayOptions> options, IHttpClientFactory httpClientFactory)
    {
        _options = options.Value.Razorpay;
        _httpClient = httpClientFactory.CreateClient("Razorpay");

        // Set up Basic Auth
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.KeyId}:{_options.KeySecret}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        _httpClient.BaseAddress = new Uri(BaseUrl);
    }

    public override PaymentGateway GatewayType => PaymentGateway.Razorpay;
    public override string DisplayName => "Razorpay";
    public override string Description => "Pay with UPI, Cards, Net Banking, or Wallets";

    public override IReadOnlyCollection<PaymentMethod> SupportedMethods =>
        [PaymentMethod.Card, PaymentMethod.UPI, PaymentMethod.NetBanking, PaymentMethod.Wallet];

    public override async Task<CreatePaymentResult> CreatePaymentAsync(
        CreatePaymentRequest request, CancellationToken ct = default)
    {
        try
        {
            var amountInPaise = (int)(request.Amount * 100);
            var receipt = $"{request.ReferenceType}_{request.ReferenceId}";

            var orderRequest = new RazorpayOrderRequest
            {
                Amount = amountInPaise,
                Currency = request.Currency,
                Receipt = receipt,
                Notes = new Dictionary<string, string>
                {
                    ["reference_id"] = request.ReferenceId.ToString(),
                    ["reference_type"] = request.ReferenceType.ToString()
                }
            };

            var response = await _httpClient.PostAsJsonAsync("/v1/orders", orderRequest, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                return new CreatePaymentResult
                {
                    Success = false,
                    ErrorMessage = $"Razorpay API error: {errorContent}",
                    Gateway = PaymentGateway.Razorpay,
                    ReferenceId = request.ReferenceId,
                    ReferenceType = request.ReferenceType
                };
            }

            var order = await response.Content.ReadFromJsonAsync<RazorpayOrderResponse>(ct);
            var orderId = order!.Id;

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
                    ["description"] = request.Description ?? $"Payment for {request.ReferenceType} #{request.ReferenceId}",
                    ["prefill_email"] = request.CustomerEmail ?? string.Empty,
                    ["prefill_contact"] = request.CustomerPhone ?? string.Empty
                },
                ReferenceId = request.ReferenceId,
                ReferenceType = request.ReferenceType
            };
        }
        catch (Exception ex)
        {
            return new CreatePaymentResult
            {
                Success = false,
                ErrorMessage = $"Failed to create Razorpay order: {ex.Message}",
                Gateway = PaymentGateway.Razorpay,
                ReferenceId = request.ReferenceId,
                ReferenceType = request.ReferenceType
            };
        }
    }

    public override Task<VerifyPaymentResult> VerifyPaymentAsync(
        VerifyPaymentRequest request, CancellationToken ct = default)
    {
        var isValid = VerifyPaymentSignature(
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

    public override async Task<PaymentStatusResult> GetPaymentStatusAsync(
        string gatewayPaymentId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/v1/payments/{gatewayPaymentId}", ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                return new PaymentStatusResult
                {
                    Success = false,
                    ErrorMessage = $"Failed to fetch payment: {errorContent}",
                    GatewayPaymentId = gatewayPaymentId,
                    Status = PaymentStatus.Pending
                };
            }

            var payment = await response.Content.ReadFromJsonAsync<RazorpayPaymentResponse>(ct);

            var status = payment!.Status switch
            {
                "captured" => PaymentStatus.Paid,
                "authorized" => PaymentStatus.Pending,
                "failed" => PaymentStatus.Failed,
                "refunded" => PaymentStatus.Refunded,
                _ => PaymentStatus.Pending
            };

            return new PaymentStatusResult
            {
                Success = true,
                GatewayPaymentId = gatewayPaymentId,
                Status = status,
                Amount = payment.Amount / 100m,
                Method = ParsePaymentMethod(payment.Method),
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds(payment.CreatedAt).UtcDateTime,
                CompletedAt = status == PaymentStatus.Paid
                    ? DateTimeOffset.FromUnixTimeSeconds(payment.CreatedAt).UtcDateTime
                    : null
            };
        }
        catch (Exception ex)
        {
            return new PaymentStatusResult
            {
                Success = false,
                ErrorMessage = $"Failed to fetch payment status: {ex.Message}",
                GatewayPaymentId = gatewayPaymentId,
                Status = PaymentStatus.Pending
            };
        }
    }

    public override async Task<RefundResult> CreateRefundAsync(
        CreateRefundRequest request, CancellationToken ct = default)
    {
        try
        {
            var amountInPaise = (int)(request.Amount * 100);

            var refundRequest = new RazorpayRefundRequest
            {
                Amount = amountInPaise,
                Notes = !string.IsNullOrEmpty(request.Reason)
                    ? new Dictionary<string, string> { ["reason"] = request.Reason }
                    : null
            };

            var response = await _httpClient.PostAsJsonAsync(
                $"/v1/payments/{request.GatewayPaymentId}/refund",
                refundRequest,
                ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                return new RefundResult
                {
                    Success = false,
                    ErrorMessage = $"Refund failed: {errorContent}",
                    GatewayPaymentId = request.GatewayPaymentId,
                    Amount = request.Amount,
                    Status = RefundStatus.Failed,
                    CreatedAt = DateTime.UtcNow
                };
            }

            var refund = await response.Content.ReadFromJsonAsync<RazorpayRefundResponse>(ct);

            var refundStatus = refund!.Status switch
            {
                "processed" => RefundStatus.Completed,
                "pending" => RefundStatus.Pending,
                "failed" => RefundStatus.Failed,
                _ => RefundStatus.Processing
            };

            return new RefundResult
            {
                Success = true,
                RefundId = refund.Id,
                GatewayPaymentId = request.GatewayPaymentId,
                Amount = request.Amount,
                Status = refundStatus,
                CreatedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            return new RefundResult
            {
                Success = false,
                ErrorMessage = $"Failed to create refund: {ex.Message}",
                GatewayPaymentId = request.GatewayPaymentId,
                Amount = request.Amount,
                Status = RefundStatus.Failed,
                CreatedAt = DateTime.UtcNow
            };
        }
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

    private bool VerifyPaymentSignature(string orderId, string paymentId, string signature)
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

    #region Razorpay API Models

    private sealed class RazorpayOrderRequest
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = "INR";

        [JsonPropertyName("receipt")]
        public string? Receipt { get; set; }

        [JsonPropertyName("notes")]
        public Dictionary<string, string>? Notes { get; set; }
    }

    private sealed class RazorpayOrderResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    private sealed class RazorpayPaymentResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("method")]
        public string? Method { get; set; }

        [JsonPropertyName("order_id")]
        public string? OrderId { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }
    }

    private sealed class RazorpayRefundRequest
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("notes")]
        public Dictionary<string, string>? Notes { get; set; }
    }

    private sealed class RazorpayRefundResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
    }

    #endregion
}

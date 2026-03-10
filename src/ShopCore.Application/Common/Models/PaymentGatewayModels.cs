namespace ShopCore.Application.Common.Models;

// ============================================
// ENUMS
// ============================================

/// <summary>
/// Type of payment reference (Order or Invoice)
/// </summary>
public enum PaymentReferenceType
{
    Order = 1,
    Invoice = 2
}

/// <summary>
/// Status of a refund operation
/// </summary>
public enum RefundStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4
}

/// <summary>
/// Types of webhook events from payment gateways
/// </summary>
public enum PaymentWebhookEventType
{
    PaymentCreated = 1,
    PaymentCaptured = 2,
    PaymentFailed = 3,
    PaymentRefunded = 4,
    RefundCreated = 5,
    RefundProcessed = 6,
    RefundFailed = 7,
    Unknown = 99
}

// ============================================
// REQUEST MODELS
// ============================================

/// <summary>
/// Generic request to create a payment
/// </summary>
public record CreatePaymentRequest
{
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "INR";
    public int ReferenceId { get; init; }
    public PaymentReferenceType ReferenceType { get; init; }
    public string? Description { get; init; }
    public string? CustomerEmail { get; init; }
    public string? CustomerPhone { get; init; }
    public string? CustomerName { get; init; }
    public IDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Generic request to verify payment
/// </summary>
public record VerifyPaymentRequest
{
    public string GatewayOrderId { get; init; } = string.Empty;
    public string GatewayPaymentId { get; init; } = string.Empty;
    public string? Signature { get; init; }
    public IDictionary<string, string>? AdditionalData { get; init; }
}

/// <summary>
/// Generic request to create refund
/// </summary>
public record CreateRefundRequest
{
    public string GatewayPaymentId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string? Reason { get; init; }
    public IDictionary<string, string>? Metadata { get; init; }
}

// ============================================
// RESULT MODELS
// ============================================

/// <summary>
/// Result of creating a payment
/// </summary>
public record CreatePaymentResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }

    public string GatewayOrderId { get; init; } = string.Empty;
    public string? GatewayPaymentId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public PaymentGateway Gateway { get; init; }

    /// <summary>
    /// Client-side data needed to complete payment.
    /// Keys vary by provider:
    /// - Razorpay: key_id, order_id, amount, currency
    /// - Stripe: publishable_key, client_secret
    /// - PayPal: client_id, order_id
    /// - COD: type, message
    /// </summary>
    public IDictionary<string, string> ClientData { get; init; } = new Dictionary<string, string>();

    public int ReferenceId { get; init; }
    public PaymentReferenceType ReferenceType { get; init; }
}

/// <summary>
/// Result of verifying a payment
/// </summary>
public record VerifyPaymentResult
{
    public bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }
    public PaymentStatus Status { get; init; }
    public string? GatewayPaymentId { get; init; }
    public PaymentMethod? Method { get; init; }
}

/// <summary>
/// Result of getting payment status
/// </summary>
public record PaymentStatusResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }

    public string GatewayPaymentId { get; init; } = string.Empty;
    public PaymentStatus Status { get; init; }
    public decimal Amount { get; init; }
    public PaymentMethod? Method { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
}

/// <summary>
/// Result of creating a refund
/// </summary>
public record RefundResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }

    public string RefundId { get; init; } = string.Empty;
    public string GatewayPaymentId { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public RefundStatus Status { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Generic webhook event parsed from any payment gateway
/// </summary>
public record PaymentWebhookEvent
{
    public PaymentWebhookEventType EventType { get; init; }
    public PaymentGateway Gateway { get; init; }
    public string GatewayPaymentId { get; init; } = string.Empty;
    public string? GatewayOrderId { get; init; }
    public decimal Amount { get; init; }
    public PaymentMethod? Method { get; init; }
    public string? RefundId { get; init; }
    public decimal? RefundAmount { get; init; }
    public int? ReferenceId { get; init; }
    public PaymentReferenceType? ReferenceType { get; init; }
    public IDictionary<string, string>? RawData { get; init; }
}

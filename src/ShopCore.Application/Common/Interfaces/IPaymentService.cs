using ShopCore.Domain.Enums;

namespace ShopCore.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<PaymentIntentResponse> CreatePaymentIntentAsync(
        decimal amount,
        int referenceId,
        PaymentReferenceType referenceType,
        string currency = "INR"
    );

    Task<bool> VerifyPaymentSignatureAsync(string razorpayOrderId, string paymentId, string signature);

    Task<PaymentGatewayResponse> GetPaymentStatusAsync(string paymentId);

    Task<RefundResponse> CreateRefundAsync(string paymentId, decimal amount, string? reason = null);

    bool VerifyWebhookSignature(string payload, string signature);

    WebhookEvent ParseWebhookPayload(string payload);
}

public enum PaymentReferenceType
{
    Order = 1,
    Invoice = 2
}

// Payment Intent (when creating payment)
public class PaymentIntentResponse
{
    public string RazorpayOrderId { get; set; } = string.Empty; // Razorpay order_id
    public string PaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string KeyId { get; set; } = string.Empty; // Razorpay Key ID for frontend
    public int ReferenceId { get; set; } // Order or Invoice ID
    public PaymentReferenceType ReferenceType { get; set; }
}

// Payment Gateway Response (when checking status)
public class PaymentGatewayResponse
{
    public string PaymentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "created", "authorized", "captured", "failed"
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty; // "card", "upi", "netbanking"
    public DateTime? CreatedAt { get; set; }
    public DateTime? CapturedAt { get; set; }

    public PaymentStatus ToDomainStatus()
    {
        return Status.ToLower() switch
        {
            "created" => PaymentStatus.Pending,
            "authorized" => PaymentStatus.Pending,
            "captured" => PaymentStatus.Paid,
            "failed" => PaymentStatus.Failed,
            "refunded" => PaymentStatus.Refunded,
            _ => PaymentStatus.Unpaid,
        };
    }
}

// Refund Response
public class RefundResponse
{
    public string RefundId { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty; // "pending", "processed", "failed"
    public DateTime CreatedAt { get; set; }
}

// Webhook Event
public class WebhookEvent
{
    public string EventType { get; set; } = string.Empty; // "payment.captured", "payment.failed", "refund.processed"
    public string PaymentId { get; set; } = string.Empty;
    public string? RazorpayOrderId { get; set; }
    public decimal Amount { get; set; }
    public string? RefundId { get; set; }
    public decimal RefundAmount { get; set; }
    public string? Method { get; set; }
    public int? ReferenceId { get; set; }
    public PaymentReferenceType? ReferenceType { get; set; }
}

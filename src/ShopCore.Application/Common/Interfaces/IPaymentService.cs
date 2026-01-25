using ShopCore.Domain.Enums;

namespace ShopCore.Application.Common.Interfaces;

public interface IPaymentService
{
    Task<PaymentIntentResponse> CreatePaymentIntentAsync(
        decimal amount,
        int orderId,
        string currency = "INR"
    );

    Task<bool> VerifyPaymentSignatureAsync(string orderId, string paymentId, string signature);

    Task<PaymentGatewayResponse> GetPaymentStatusAsync(string paymentId);
}

// Payment Intent (when creating payment)
public class PaymentIntentResponse
{
    public string OrderId { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string KeyId { get; set; } = string.Empty; // Razorpay Key ID
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

    // Helper to convert to our domain enum
    public PaymentStatus ToDomainStatus()
    {
        return Status.ToLower() switch
        {
            "created" => Domain.Enums.PaymentStatus.Pending,
            "authorized" => Domain.Enums.PaymentStatus.Pending,
            "captured" => Domain.Enums.PaymentStatus.Paid,
            "failed" => Domain.Enums.PaymentStatus.Failed,
            "refunded" => Domain.Enums.PaymentStatus.Refunded,
            _ => Domain.Enums.PaymentStatus.Unpaid,
        };
    }
}

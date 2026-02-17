using ShopCore.Domain.Enums;

namespace ShopCore.Infrastructure.PaymentGateways;

/// <summary>
/// Configuration options for payment gateways
/// </summary>
public class PaymentGatewayOptions
{
    public const string SectionName = "PaymentGateways";

    /// <summary>
    /// Default gateway to use when not specified
    /// </summary>
    public PaymentGateway DefaultGateway { get; set; } = PaymentGateway.Razorpay;

    /// <summary>
    /// List of enabled gateways
    /// </summary>
    public List<PaymentGateway> EnabledGateways { get; set; } = new() { PaymentGateway.Razorpay };

    /// <summary>
    /// Enable Cash on Delivery option
    /// </summary>
    public bool EnableCOD { get; set; } = true;

    /// <summary>
    /// Razorpay-specific settings
    /// </summary>
    public RazorpayOptions Razorpay { get; set; } = new();

    /// <summary>
    /// Stripe-specific settings
    /// </summary>
    public StripeOptions Stripe { get; set; } = new();

    /// <summary>
    /// PayPal-specific settings
    /// </summary>
    public PayPalOptions PayPal { get; set; } = new();
}

/// <summary>
/// Razorpay configuration
/// </summary>
public class RazorpayOptions
{
    public string KeyId { get; set; } = string.Empty;
    public string KeySecret { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}

/// <summary>
/// Stripe configuration
/// </summary>
public class StripeOptions
{
    public string PublishableKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
}

/// <summary>
/// PayPal configuration
/// </summary>
public class PayPalOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public bool UseSandbox { get; set; } = true;
    public string WebhookId { get; set; } = string.Empty;
}
